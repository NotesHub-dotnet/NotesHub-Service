using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NotesHub_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class NotesHubService : INotesHubService
    {
        public async Task<UserModel> GetUserById(int value)
        {
            noteshubEntities db = new noteshubEntities();

            User user1 = await db.Users.FindAsync(value);
            UserModel model = new UserModel();
            model.id = user1.id;
            model.name = user1.name;
            model.email = user1.email;
            model.username = user1.username;
            model.last_viewed = user1.last_viewed;
            model.created_at = user1.created_at;
            model.password = user1.password;


            return model;
        }

        public int Signup(UserModel userData)
        {
            noteshubEntities db = new noteshubEntities();
            bool isUserExists = db.Users.Any(user => user.username == userData.username);
            if (isUserExists)
            {
                return 0;
            }
            User newUser = new User();
            newUser.name = userData.name;
            newUser.email = userData.email;
            newUser.username = userData.username;
            newUser.last_viewed = DateTime.Now;
            newUser.created_at = DateTime.Now;
            newUser.password = userData.password;
            db.Users.Add(newUser);
            db.SaveChanges();

            return newUser.id;
        }
        public async Task<int> Login(UserModel userData)
        {
            noteshubEntities db = new noteshubEntities();
            User isUserExists = await db.Users.FirstOrDefaultAsync(user => user.username == userData.username && user.password == userData.password);
            if(isUserExists == null)
            {
                return 0;
            }
            return isUserExists.id;
        }

        ////group
        public async Task<GroupModel> GetGroupById(int id)
        {
            noteshubEntities db = new noteshubEntities();

            Group groupData = await db.Groups.FindAsync(id);
            GroupModel model = new GroupModel();
            model.id = groupData.id;
            model.name = groupData.name;
            model.created_at = groupData.created_at;
            model.description = groupData.description;

            return model;
        }

        public async Task<IEnumerable<GroupModel>> GetAllGroup()
        {
            noteshubEntities db = new noteshubEntities();

            List<Group> groupData = await db.Groups.ToListAsync();

            List<GroupModel> response = new List<GroupModel>();
            foreach (var item in groupData)
            {
                GroupModel model = new GroupModel();
                model.id = item.id;
                model.name = item.name;
                model.created_at = item.created_at;
                model.description = item.description;
                response.Add(model);
            }
            return response;
        }

        public async Task<int> CreateGroup(GroupModel groupData)
        {

            noteshubEntities db = new noteshubEntities();

            Group newGroup = new Group();
            newGroup.name = groupData.name;
            newGroup.created_at = DateTime.Now;
            newGroup.description = groupData.description;

            db.Groups.Add(newGroup);
            db.SaveChanges();
            return newGroup.id;
        }


        public async Task<int> JoinGroup(GroupUserModel groupUser)
        {

            noteshubEntities db = new noteshubEntities();

            GroupUser newGroupUser = new GroupUser();

            newGroupUser.user_id = groupUser.user_id;
            newGroupUser.group_id = groupUser.group_id;

            db.GroupUsers.Add(newGroupUser);
            db.SaveChanges();
            return newGroupUser.id;
        }

        public async Task<bool> LeaveGroup(GroupUserModel groupUserData)
        {
            noteshubEntities db = new noteshubEntities();
            GroupUser dbGroupUser = await db.GroupUsers.FirstOrDefaultAsync(groupUser => groupUser.group_id == groupUserData.group_id && groupUser.user_id == groupUserData.user_id);

            if (dbGroupUser != null)
            {
                db.GroupUsers.Remove(dbGroupUser);
                db.SaveChanges();
                return true;
            }

            return false;

        }

        public async Task<IEnumerable<UserModel>> GetAllGroupUsers(int groupId)
        {


            noteshubEntities db = new noteshubEntities();

            List<User> userData = await db.Users.Include(user => user.GroupUsers).ToListAsync();
            List<UserModel> response = new List<UserModel>();
            foreach (var item in userData)
            {
                if (item.GroupUsers.Any(gUser => gUser.group_id == groupId))
                {
                    UserModel model = new UserModel();
                    model.id = item.id;
                    model.name = item.name;
                    model.email = item.email;
                    model.username = item.username;
                    model.last_viewed = item.last_viewed;
                    model.created_at = item.created_at;
                    model.password = item.password;
                    response.Add(model);
                }
            }
            return response;
        }

        public async Task<IEnumerable<GroupModel>> GetAllGroupByUserId(int userId)
        {


            noteshubEntities db = new noteshubEntities();

            List<Group> groupData = await db.Groups.Include(user => user.GroupUsers).ToListAsync();
            List<GroupModel> response = new List<GroupModel>();
            foreach (var item in groupData)
            {
                if (item.GroupUsers.Any(gUser => gUser.user_id == userId))
                {
                    GroupModel model = new GroupModel();
                    model.id = item.id;
                    model.name = item.name;
                    model.created_at = item.created_at;
                    model.description = item.description;
                }
            }
            return response;
        }

        //post
        public async Task<PostModel> GetPostById(int id)
        {
            noteshubEntities db = new noteshubEntities();

            Post postData = await db.Posts.FindAsync(id);
            PostModel model = new PostModel();
            model.id = postData.id;
            model.group_id = postData.group_id;
            model.user_id = postData.user_id;
            model.description = postData.description;
            model.created_at = postData.created_at;
            model.title = postData.title;
            List<Document> documents = db.Documents.Where(document => document.post_id == id).ToList();
            model.Documents = new List<DocumentModel>();
            foreach (var item in documents)
            {
                model.Documents.Add(new DocumentModel { fileBytes = FileService.GetFile(item.file_path), name = item.name });
            }

            return model;

        }

        public async Task<int> CreatePost(PostModel postData)
        {
            noteshubEntities db = new noteshubEntities();



            Post newPost = new Post();

            newPost.title = postData.title;
            newPost.created_at = DateTime.Now;
            newPost.description = postData.description;
            newPost.group_id = postData.group_id;
            newPost.user_id = postData.user_id;
            db.Posts.Add(newPost);
            db.SaveChanges();
            int postId = newPost.id;
            int i = 0;
            if(postData.Documents != null)
            {

                foreach (var item in postData.Documents)
                {
                    string filePath = FileService.UploadFile(postId.ToString() + i.ToString(), item.fileBytes);
                    Document document = new Document();
                    document.name = item.name;
                    document.post_id = postId;
                    document.file_path = filePath;
                    db.Documents.Add(document);
                    db.SaveChanges();
                    i++;
                }
            }

            return postId;
        }
        public async Task<bool> DeletePost(int id)
        {
            noteshubEntities db = new noteshubEntities();

            Post postData = await db.Posts.FindAsync(id);
            db.Posts.Remove(postData);
            await db.SaveChangesAsync();
            return true;

        }

        public async Task<IEnumerable<PostModel>> GetAllPost()
        {

            noteshubEntities db = new noteshubEntities();

            List<Post> postData = await db.Posts.ToListAsync();
            List<PostModel> response = new List<PostModel>();
            foreach (var item in postData)
            {
                PostModel model = new PostModel();
                model.id = item.id;
                model.group_id = item.group_id;
                model.user_id = item.user_id;
                model.description = item.description;
                model.created_at = item.created_at;
                model.title = item.title;
                List<Document> documents = db.Documents.Where(document => document.post_id == item.id).ToList();
                foreach (var item2 in documents)
                {
                    model.Documents.Add(new DocumentModel { fileBytes = FileService.GetFile(item2.file_path), name = item2.name });
                }
                response.Add(model);
            }
            return response;
        }


        public async Task<IEnumerable<PostModel>> GetAllPostByGroupId(int groupId)
        {
            noteshubEntities db = new noteshubEntities();

            List<Post> postData = await db.Posts.Where(post => post.group_id == groupId).ToListAsync();
            List<PostModel> response = new List<PostModel>();
            foreach (var item in postData)
            {
                PostModel model = new PostModel();
                model.id = item.id;
                model.group_id = item.group_id;
                model.user_id = item.user_id;
                model.description = item.description;
                model.created_at = item.created_at;
                model.title = item.title;
                List<Document> documents = db.Documents.Where(document => document.post_id == item.id).ToList();
                model.Documents = new List<DocumentModel>();
                foreach (var item2 in documents)
                {
                    model.Documents.Add(new DocumentModel { fileBytes = FileService.GetFile(item2.file_path), name = item2.name });
                }
                response.Add(model);
            }
            return response;

        }


        public async Task<IEnumerable<PostModel>> GetAllPostByUserId(int userId)
        {

            noteshubEntities db = new noteshubEntities();

            List<Post> postData = await db.Posts.Where(post => post.user_id == userId).ToListAsync();
            List<PostModel> response = new List<PostModel>();
            foreach (var item in postData)
            {
                PostModel model = new PostModel();
                model.id = item.id;
                model.group_id = item.group_id;
                model.user_id = item.user_id;
                model.description = item.description;
                model.created_at = item.created_at;
                model.title = item.title;
                List<Document> documents = db.Documents.Where(document => document.post_id == item.id).ToList();
                foreach (var item2 in documents)
                {
                    model.Documents.Add(new DocumentModel { fileBytes = FileService.GetFile(item2.file_path), name = item2.name });
                }
                response.Add(model);
            }
            return response;
        }


    }
}
