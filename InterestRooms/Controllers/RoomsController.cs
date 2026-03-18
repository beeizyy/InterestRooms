using InterestRooms.Data;
using InterestRooms.Models;
using InterestRooms.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterestRooms.Controllers
{
    public class RoomsController : Controller
    {
        private readonly AppDbContext _context;

        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var rooms = _context.Rooms.ToList();
            return View(rooms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateRoomViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var room = new Room
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                CreatedByUserId = userId.Value,
                CreatedAt = DateTime.Now,
                IsPrivate = false
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            
            var membership = new RoomMember
            {
                RoomId = room.Id,
                UserId = userId.Value,
                JoinedAt = DateTime.Now
            };

            var ownerNickname = new RoomNickname
            {
                RoomId = room.Id,
                UserId = userId.Value,
                Nickname = "Owner"
            };

            _context.RoomMembers.Add(membership);
            _context.RoomNicknames.Add(ownerNickname);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = room.Id });
        }

        [HttpGet]
        public IActionResult Join(int roomId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var room = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null)
                return NotFound();

            bool alreadyMember = _context.RoomMembers.Any(rm => rm.RoomId == roomId && rm.UserId == userId.Value);
            if (alreadyMember)
                return RedirectToAction("Details", new { id = roomId });

            var model = new JoinRoomViewModel
            {
                RoomId = room.Id,
                RoomName = room.Name
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Join(JoinRoomViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var room = _context.Rooms.FirstOrDefault(r => r.Id == model.RoomId);
            if (room == null)
                return NotFound();

            bool alreadyMember = _context.RoomMembers.Any(rm => rm.RoomId == model.RoomId && rm.UserId == userId.Value);
            if (alreadyMember)
                return RedirectToAction("Details", new { id = model.RoomId });

            if (!ModelState.IsValid)
            {
                model.RoomName = room.Name;
                return View(model);
            }

            bool nicknameExists = _context.RoomNicknames.Any(rn => rn.RoomId == model.RoomId && rn.Nickname == model.Nickname);
            if (nicknameExists)
            {
                ModelState.AddModelError("", "This nickname is already taken in this room.");
                model.RoomName = room.Name;
                return View(model);
            }

            bool alreadyRequested = _context.JoinRequests.Any(jr =>
                jr.RoomId == model.RoomId &&
                jr.UserId == userId.Value &&
                jr.Status == "Pending");

            if (alreadyRequested)
            {
                ModelState.AddModelError("", "You already have a pending join request for this room.");
                model.RoomName = room.Name;
                return View(model);
            }

            var request = new JoinRequest
            {
                RoomId = model.RoomId,
                UserId = userId.Value,
                Nickname = model.Nickname,
                RequestedAt = DateTime.Now,
                Status = "Pending"
            };

            _context.JoinRequests.Add(request);
            _context.SaveChanges();

            TempData["Message"] = "Join request sent. Waiting for room owner approval.";
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            bool isMember = _context.RoomMembers.Any(rm => rm.RoomId == id && rm.UserId == userId.Value);
            if (!isMember)
                return RedirectToAction("Index");

            var room = _context.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null)
                return NotFound();

            var rawMessages = _context.Messages
                .Where(m => m.RoomId == id && !m.IsDeleted)
                .OrderByDescending(m => m.SentAt)
                .ToList();

            var nicknames = _context.RoomNicknames
                .Where(rn => rn.RoomId == id)
                .ToDictionary(rn => rn.UserId, rn => rn.Nickname);

            var allLikes = _context.MessageLikes
                .Where(ml => rawMessages.Select(m => m.Id).Contains(ml.MessageId))
                .ToList();

            var messages = rawMessages.Select(m =>
            {
                var replyMessage = m.ReplyToMessageId.HasValue
                    ? rawMessages.FirstOrDefault(x => x.Id == m.ReplyToMessageId.Value)
                    : null;

                int likeCount = allLikes.Count(ml => ml.MessageId == m.Id);
                bool isLikedByCurrentUser = allLikes.Any(ml => ml.MessageId == m.Id && ml.UserId == userId.Value);

                return new MessageDisplayViewModel
                {
                    MessageId = m.Id,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    Nickname = nicknames.ContainsKey(m.UserId) ? nicknames[m.UserId] : "Anonymous",
                    ProfileImagePath = _context.Users
                        .Where(u => u.Id == m.UserId)
                        .Select(u => u.ProfileImagePath)
                        .FirstOrDefault(),
                    ReplyToMessageId = m.ReplyToMessageId,
                    ReplyToContent = replyMessage?.Content,
                    ReplyToNickname = replyMessage != null && nicknames.ContainsKey(replyMessage.UserId)
                        ? nicknames[replyMessage.UserId]
                        : null,
                    LikeCount = likeCount,
                    IsLikedByCurrentUser = isLikedByCurrentUser,
                    IsOwnMessage = m.UserId == userId.Value
                };
            }).ToList();

            var viewModel = new RoomDetailsViewModel
            {
                Room = room,
                Messages = messages
            };

            return View(viewModel);
        }
        public IActionResult Requests(int roomId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var room = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null)
                return NotFound();

            if (room.CreatedByUserId != userId.Value)
                return RedirectToAction("Index");

            var requests = _context.JoinRequests
                .Where(jr => jr.RoomId == roomId && jr.Status == "Pending")
                .Join(_context.Users,
                      jr => jr.UserId,
                      u => u.Id,
                      (jr, u) => new InterestRooms.ViewModels.JoinRequestDisplayViewModel
                      {
                          Id = jr.Id,
                          RoomId = jr.RoomId,
                          Username = u.Username,
                          Email = u.Email,
                          Nickname = jr.Nickname,
                          RequestedAt = jr.RequestedAt
                      })
                .ToList();

            ViewBag.RoomName = room.Name;
            ViewBag.RoomId = room.Id;

            return View(requests);
        }

        [HttpPost]
        public IActionResult ApproveRequest(int requestId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var request = _context.JoinRequests.FirstOrDefault(jr => jr.Id == requestId);
            if (request == null)
                return NotFound();

            var room = _context.Rooms.FirstOrDefault(r => r.Id == request.RoomId);
            if (room == null)
                return NotFound();

            if (room.CreatedByUserId != userId.Value)
                return RedirectToAction("Index");

            bool alreadyMember = _context.RoomMembers.Any(rm => rm.RoomId == request.RoomId && rm.UserId == request.UserId);

            if (!alreadyMember)
            {
                var membership = new RoomMember
                {
                    RoomId = request.RoomId,
                    UserId = request.UserId,
                    JoinedAt = DateTime.Now
                };

                var roomNickname = new RoomNickname
                {
                    RoomId = request.RoomId,
                    UserId = request.UserId,
                    Nickname = request.Nickname
                };

                _context.RoomMembers.Add(membership);
                _context.RoomNicknames.Add(roomNickname);
            }

            request.Status = "Approved";
            _context.SaveChanges();

            return RedirectToAction("Requests", new { roomId = request.RoomId });
        }

        [HttpPost]
        public IActionResult RejectRequest(int requestId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var request = _context.JoinRequests.FirstOrDefault(jr => jr.Id == requestId);
            if (request == null)
                return NotFound();

            var room = _context.Rooms.FirstOrDefault(r => r.Id == request.RoomId);
            if (room == null)
                return NotFound();

            if (room.CreatedByUserId != userId.Value)
                return RedirectToAction("Index");

            request.Status = "Rejected";
            _context.SaveChanges();

            return RedirectToAction("Requests", new { roomId = request.RoomId });
        }



        [HttpPost]
        public IActionResult SendMessage(int roomId, string newMessageContent, int? replyToMessageId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            bool isMember = _context.RoomMembers.Any(rm => rm.RoomId == roomId && rm.UserId == userId.Value);
            if (!isMember)
                return RedirectToAction("Index");

            if (string.IsNullOrWhiteSpace(newMessageContent))
                return RedirectToAction("Details", new { id = roomId });

            var message = new Message
            {
                RoomId = roomId,
                UserId = userId.Value,
                Content = newMessageContent,
                SentAt = DateTime.Now,
                IsDeleted = false,
                ReplyToMessageId = replyToMessageId
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = roomId });
        }

        public IActionResult MyRooms()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var myRooms = _context.RoomMembers
                .Where(rm => rm.UserId == userId.Value)
                .Select(rm => rm.Room)
                .ToList();

            return View(myRooms);
        }

        public IActionResult ToggleLike(int roomId, int messageId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            bool isMember = _context.RoomMembers.Any(rm => rm.RoomId == roomId && rm.UserId == userId.Value);
            if (!isMember)
                return RedirectToAction("Index");

            var existingLike = _context.MessageLikes
                .FirstOrDefault(ml => ml.MessageId == messageId && ml.UserId == userId.Value);

            if (existingLike == null)
            {
                var like = new MessageLike
                {
                    MessageId = messageId,
                    UserId = userId.Value
                };

                _context.MessageLikes.Add(like);
            }
            else
            {
                _context.MessageLikes.Remove(existingLike);
            }

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = roomId });
        }
        [HttpPost]
        public IActionResult Delete(int roomId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var room = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null)
                return NotFound();

            if (room.CreatedByUserId != userId.Value)
                return RedirectToAction("Index");

            _context.Rooms.Remove(room);
            _context.SaveChanges();

            return RedirectToAction("MyRooms");
        }
    }
}