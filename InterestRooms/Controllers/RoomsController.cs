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

            return RedirectToAction("Index");
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

            var membership = new RoomMember
            {
                RoomId = model.RoomId,
                UserId = userId.Value,
                JoinedAt = DateTime.Now
            };

            var roomNickname = new RoomNickname
            {
                RoomId = model.RoomId,
                UserId = userId.Value,
                Nickname = model.Nickname
            };

            _context.RoomMembers.Add(membership);
            _context.RoomNicknames.Add(roomNickname);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = model.RoomId });
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
    }
}