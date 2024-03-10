using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.RightsManagement;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NotesHub_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface INotesHubService
    {
        // user 
        [OperationContract]
        Task<UserModel> GetUserById(int id);

        [OperationContract]
        int Signup(UserModel user);

        [OperationContract]
        Task<int> Login(UserModel user);


        ////group
       
        [OperationContract]
        Task<GroupModel> GetGroupById(int id);

        [OperationContract]
        Task<IEnumerable<GroupModel>> GetAllGroup();

        [OperationContract]
        Task<int> CreateGroup(GroupModel group);


        ////group management

        [OperationContract]
        Task<int> JoinGroup(GroupUserModel groupUser);

        [OperationContract]
        Task<bool> LeaveGroup(GroupUserModel groupUser);

        [OperationContract]
        Task<IEnumerable<UserModel>> GetAllGroupUsers(int groupId);

        [OperationContract]
        Task<IEnumerable<GroupModel>> GetAllGroupByUserId(int userId);

        ////post
        [OperationContract]
        Task<PostModel> GetPostById(int id);

        [OperationContract]
        Task<int> CreatePost(PostModel post);
        [OperationContract]
        Task<bool> DeletePost(int id);

        [OperationContract]
        Task<IEnumerable<PostModel>> GetAllPost();


        [OperationContract]
        Task<IEnumerable<PostModel>> GetAllPostByGroupId(int groupId);


        [OperationContract]
        Task<IEnumerable<PostModel>> GetAllPostByUserId(int userId);

        // TODO: Add your service operations here
    }

    [DataContract]
    public class DocumentModel
    {
        [DataMember]
        public string name { get; set; }

        [DataMember]
        public byte[] fileBytes { get; set; }
    }

    [DataContract]
    public class UserModel
    {


        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string username { get; set; }

        [DataMember]
        public string password { get; set; }

        [DataMember]
        public Nullable<System.DateTime> last_viewed { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public Nullable<System.DateTime> created_at { get; set; }
    }

    [DataContract]
    public class GroupUserModel
    {

        [DataMember]
        public int id { get; set; }

        [DataMember]
        public Nullable<int> user_id { get; set; }

        [DataMember]
        public Nullable<int> group_id { get; set; }
    }

    [DataContract]
    public class PostModel
    {

        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public Nullable<int> user_id { get; set; }
        [DataMember]
        public Nullable<int> group_id { get; set; }
        [DataMember]
        public Nullable<System.DateTime> created_at { get; set; }

        [DataMember]
        public List<DocumentModel> Documents { get; set; }
    }

    [DataContract]

    public class GroupModel
    {

        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public Nullable<System.DateTime> created_at { get; set; }

    }
}
