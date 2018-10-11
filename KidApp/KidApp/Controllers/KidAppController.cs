using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KidApp.Controllers
{
    public class Status
    {
        public int code { get; set; }
        public string message { get; set; }
        public Status(int code, string message)
        {
            this.code = code;
            this.message = message;
        }

        public Status()
        {
        }
    }

    public class ObjectResponse
    {
        public Status status = new Status(200, "Success");
        public Dictionary<String, dynamic> data { get; set; }
    }

    [RoutePrefix("api")]
    public class KidAppController : ApiController
    {
        KidAppDBDataContext db = new KidAppDBDataContext();

        private HttpResponseMessage ErrorResponseMessage(string message)
        {
            ObjectResponse response = new ObjectResponse();
            response.status = new Status(0, message);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        /// <summaryRegion description="All functions in region User"> 
        /// <function name="GetUsernameById(string id)"><return value="string"></return></function>
        /// <function name="ExistedUser(string username)"><return value="Bool"></return></function>
        /// <function name="PostLogin()"><return value="HttpResponseMessage"></return></function>
        /// <function name="PostRegister()"><return value="HttpResponseMessage"></return></function>
        /// <function name="PutChangePassword()"><return value="HttpResponseMessage"></return></function>
        /// </summaryRegion>
        #region User
        /// <summary>
        /// Private Function: Get Username by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>String Username if existed this id in DB or null if not</returns>
        private string GetUsernameById(string id)
        {
            var user = from us in db.Users
                       where us.userId == id
                       select us;
            if (user.Count() == 0) return null;
            return user.First().userName;
        }

        /// <summary>
        /// Private Function: Check if user is created in DB or not
        /// </summary>
        /// <param name="username"></param>
        /// <returns>True if user existed or False if user still not existed </returns>
        private bool ExistedUser(string username)
        {
            var user = from us in db.Users
                       where us.userName == username
                       select us;
            if (user.Count() == 0) return false;
            return true;
        }

        /// <summary>
        /// Function: Log in by Username and Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>access_token, name, email, phone</returns>
        [Route("login")]
        public HttpResponseMessage PostLogin()
        {
            try
            {
                var username = System.Web.HttpContext.Current.Request.Params["username"];
                var password = System.Web.HttpContext.Current.Request.Params["password"];

                byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
                data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                String passwordHash = System.Text.Encoding.ASCII.GetString(data);

                var user = from us in db.Users
                           where us.userName == username && us.password == passwordHash
                           select us;
                if (user.Count() == 0)
                {
                    return ErrorResponseMessage("Sai tên đăng nhập hoặc mật khẩu");
                }
                else if (user.First().active == false)
                {
                    return ErrorResponseMessage("Tài khoản đã bị vô hiệu hóa. Vui lòng sử dụng tài khoản khác.");
                }
                else
                {
                    ObjectResponse response = new ObjectResponse
                    {
                        status = new Status(200, "Đăng nhập thành công"),
                        data = new Dictionary<string, dynamic>()
                    };
                    response.data.Add("username", user.First().userName);
                    response.data.Add("date_create", user.First().dob);
                    response.data.Add("address", user.First().address);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            } catch (Exception ex)
            {
                return ErrorResponseMessage("Có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Function: Register new account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="address"></param>
        /// <returns>access_token, name, email, phone</returns>
        [Route("register")]
        public HttpResponseMessage PostRegister()
        {
            try
            {
                var username = System.Web.HttpContext.Current.Request.Params["username"];
                var password = System.Web.HttpContext.Current.Request.Params["password"];
                var address = System.Web.HttpContext.Current.Request.Params["address"];

                if (!username.All(b => b < 128) || !password.All(b => b < 128)) // Check ASCII character.
                {
                    return ErrorResponseMessage("Tên đăng nhập hoặc mật khẩu không đúng định dạng.");
                }
                else if (username.Count() == 0 || password.Count() == 0 || address.Count() == 0) // Check enough information.
                {
                    return ErrorResponseMessage("Vui lòng nhập đầy đủ thông tin.");
                }
                else if (username.Length < 6 || password.Length < 6) // Check length of username and password.
                {
                    return ErrorResponseMessage("Tên đăng nhập và mật khẩu phải có độ dài ít nhất 6 ký tự");
                }
                else
                {
                    if (ExistedUser(username))
                    {
                        return ErrorResponseMessage("Tên đăng nhập đã tồn tại. Vui lòng chọn tên đăng nhập khác.");
                    }
                    byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
                    data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                    String passwordHash = System.Text.Encoding.ASCII.GetString(data);

                    var lastUser = (from us in db.Users orderby us.userId descending select us.userId).First();
                    int p = int.Parse(lastUser.Substring(4)) + 1;
                    string id = "User" + p.ToString();
                    User user = new User
                    {
                        userId = id,
                        userName = username,
                        password = passwordHash,
                        address = address,
                        dob = DateTime.Now,
                        active = true
                    };
                    db.Users.InsertOnSubmit(user);
                    db.SubmitChanges();
                    ObjectResponse response = new ObjectResponse
                    {
                        status = new Status(200, "Đăng ký thành công"),
                        data = new Dictionary<string, dynamic>()
                    };
                    response.data.Add("username", user.userName);
                    response.data.Add("date_create", user.dob);
                    response.data.Add("address", user.address);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            } catch (Exception ex)
            {
                return ErrorResponseMessage("Có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Function: Update account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns>access_token, name, email, phone</returns>
        [Route("changePassword")]
        public HttpResponseMessage PutChangePassword()
        {
            try { 
                var username = System.Web.HttpContext.Current.Request.Params["username"];
                var oldPassword = System.Web.HttpContext.Current.Request.Params["old_password"];
                var newPassword = System.Web.HttpContext.Current.Request.Params["new_password"];

                if (username.Count() == 0 || oldPassword.Count() == 0 || newPassword.Count() == 0)
                {
                    return ErrorResponseMessage("Vui lòng nhập đầy đủ thông tin.");
                }
                if (newPassword.Length < 6)
                {
                    return ErrorResponseMessage("Mật khẩu mới phải có ít nhất 6 ký tự.");
                }
                User user = db.Users.Single(us => us.userName == username);
                if (user.userId == null || user.active == false)
                {
                    return ErrorResponseMessage("Tài khoản không hợp lệ.");
                }
                byte[] data = System.Text.Encoding.ASCII.GetBytes(oldPassword);
                data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                String oldPasswordHash = System.Text.Encoding.ASCII.GetString(data);
                if (oldPasswordHash != user.password)
                {
                    return ErrorResponseMessage("Sai mật khẩu xác nhận.");
                }

                byte[] newData = System.Text.Encoding.ASCII.GetBytes(newPassword);
                newData = new System.Security.Cryptography.SHA256Managed().ComputeHash(newData);
                String newPasswordHash = System.Text.Encoding.ASCII.GetString(newData);

                user.password = newPasswordHash;
                db.SubmitChanges();
                ObjectResponse response = new ObjectResponse
                {
                    status = new Status(200, "Cập nhật mật khẩu thành công."),
                    data = new Dictionary<string, dynamic>()
                };
                response.data.Add("username", user.userName);
                response.data.Add("date_create", user.dob);
                response.data.Add("address", user.address);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            } catch (Exception)
            {
                return ErrorResponseMessage("Có lỗi xảy ra. Vui lòng thử lại sau");
            }
        }

        #endregion User

        /// <summaryRegion description="All functions in region Image"> 
        /// <function name="PostAddImage()"><return value="HttpResponseMessage"></return></function>
        /// <function name="DeleteRemoveImage()"><return value="HttpResponseMessage"></return></function>
        /// <function name="GetLastImage()"><return value="HttpResponseMessage"></return></function>
        /// </summaryRegion>
        #region Image
        /// <summary>
        /// Function: Add Image
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="username"></param>
        /// <returns>imageId, imageName, timeShoot, userId</returns>
        [Route("addImage")]
        public HttpResponseMessage PostAddImage()
        {
            try
            {
                var name = System.Web.HttpContext.Current.Request.Params["name"];
                var time = System.Web.HttpContext.Current.Request.Params["time"];
                var username = System.Web.HttpContext.Current.Request.Params["username"];


                if (!ExistedUser(username))
                {
                    return ErrorResponseMessage("Người dùng không tồn tại.");
                }
                else
                {
                    var user = from us in db.Users
                               where us.userName == username
                               select us.userId;

                    var lastImage = (from im in db.Images
                                     orderby im.imageId
                                     select im.imageId).First();
                    int p = int.Parse(lastImage.Substring(5)) + 1;
                    string id = "Image" + p.ToString();
                    Image image = new Image
                    {
                        imageId = id,
                        imageName = name,
                        timeShoot = Double.Parse(time),
                        userId = user.First(),
                        active = true
                    };
                    db.Images.InsertOnSubmit(image);
                    db.SubmitChanges();
                    ObjectResponse response = new ObjectResponse
                    {
                        status = new Status(200, "Thêm vào thành công"),
                        data = new Dictionary<string, dynamic>()
                    };
                    response.data.Add("image_id", image.imageId);
                    response.data.Add("image_name", image.imageName);
                    response.data.Add("time_shoot", image.timeShoot);
                    response.data.Add("user_id", image.userId);
                    response.data.Add("active", image.active);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                return ErrorResponseMessage("Có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Function: Remove Image
        /// </summary>
        /// <param name="image_id"></param>
        /// <returns>success or fail</returns>
        [Route("removeImage")]
        public HttpResponseMessage DeleteRemoveImage()
        {
            try
            {
                var id = System.Web.HttpContext.Current.Request.Params["image_id"];

                var image = from im in db.Images
                            where im.imageId == id
                            select im;
                if (image.Count() == 0)
                {
                    return ErrorResponseMessage("Hình ảnh không tồn tại.");
                }
                image.First().active = false;
                db.SubmitChanges();
                ObjectResponse response = new ObjectResponse
                {
                    status = new Status(200, "Xóa hình ảnh thành công"),
                    data = new Dictionary<string, dynamic>()
                };
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponseMessage("Có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Function: Get Last Image
        /// </summary>
        /// <returns>always success</returns>
        [Route("getLastImage")]
        public HttpResponseMessage GetLastImage()
        {
            try
            {
                var image = (from im in db.Images
                                    where im.active == true
                                    orderby im.imageId descending
                                    select im).First();
                ObjectResponse response = new ObjectResponse
                {
                    status = new Status(200, "OK"),
                    data = new Dictionary<string, dynamic>()
                };
                response.data.Add("image_id", image.imageId);
                response.data.Add("image_name", image.imageName);
                response.data.Add("time_shoot", image.timeShoot);
                response.data.Add("user_id", image.userId);
                response.data.Add("active", image.active);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponseMessage("Có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }

        #endregion Image

        /// <summaryRegion description="All functions in region EngResult"> 
        /// <function name="PostAddEngResult()"><return value="HttpResponseMessage"></return></function>
        /// <function name="PostRemoveEngResult()"><return value="HttpResponseMessage"></return></function>
        /// </summaryRegion>
        #region EngResult

        /// <summary>
        /// Function: Add A new or existed the engsub of a Image
        /// </summary>
        /// <returns>eng_id, eng_1, eng_2, eng_3</returns>
        [Route("addEngResult")]
        public HttpResponseMessage PostAddEngResult()
        {
            try
            {
                var id = System.Web.HttpContext.Current.Request.Params["image_id"];
                var eng1 = System.Web.HttpContext.Current.Request.Params["eng_1"];
                var eng2 = System.Web.HttpContext.Current.Request.Params["eng_2"];
                var eng3 = System.Web.HttpContext.Current.Request.Params["eng_3"];

                var image = from im in db.Images
                            where im.imageId == id
                            select im;
                if (image.Count() == 0)
                {
                    return ErrorResponseMessage("Hình ảnh không hợp lệ.");
                }
                var result = from im in db.EngResults
                            where im.engId == id
                            select im;
                if (result.Count() == 0)
                {
                    EngResult newResult = new EngResult
                    {
                        engId = id,
                        eng1 = eng1,
                        eng2 = eng2,
                        eng3 = eng3,
                        active = true                        
                    };
                    db.EngResults.InsertOnSubmit(newResult);
                    db.SubmitChanges();
                    ObjectResponse response = new ObjectResponse
                    {
                        status = new Status(200, "Thêm vào thành công"),
                        data = new Dictionary<string, dynamic>()
                    };
                    response.data.Add("eng_id", newResult.engId);
                    response.data.Add("eng_1", newResult.eng1);
                    response.data.Add("eng_2", newResult.eng2);
                    response.data.Add("eng_3", newResult.eng3);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                } else
                {
                    result.First().eng1 = eng1;
                    result.First().eng2 = eng2;
                    result.First().eng3 = eng3;
                    result.First().active = true;
                    db.SubmitChanges();
                    ObjectResponse response = new ObjectResponse
                    {
                        status = new Status(200, "Ghi đè thành công"),
                        data = new Dictionary<string, dynamic>()
                    };
                    response.data.Add("eng_id", result.First().engId);
                    response.data.Add("eng_1", result.First().eng1);
                    response.data.Add("eng_2", result.First().eng2);
                    response.data.Add("eng_3", result.First().eng3);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                return ErrorResponseMessage("Có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Function: Remove an EngResult
        /// </summary>
        /// <returns>success or fail</returns>
        [Route("removeEngResult")]
        public HttpResponseMessage DeleteRemoveEngResult()
        {
            try
            {
                var id = System.Web.HttpContext.Current.Request.Params["eng_id"];
                var result = from im in db.EngResults
                             where im.engId == id
                             select im;
                if (result.Count() == 0)
                {
                    return ErrorResponseMessage("Không tìm thấy kết quả.");
                }
                else
                {
                    result.First().active = false;
                    db.SubmitChanges();
                    ObjectResponse response = new ObjectResponse
                    {
                        status = new Status(200, "Xóa thành công"),
                        data = new Dictionary<string, dynamic>()
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, response); ;
                }
            }
            catch (Exception ex)
            {
                return ErrorResponseMessage("Có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }

        #endregion EngResult

        /// <summaryRegion description="All functions in region VieResult"> 
        /// <function name="PostAddEngResult()"><return value="HttpResponseMessage"></return></function>
        /// <function name="PostRemoveEngResult()"><return value="HttpResponseMessage"></return></function>
        /// </summaryRegion>
        #region ViewResult

        /// <summary>
        /// Function: Add A new or existed the viesub of a Image
        /// </summary>
        /// <returns>vie_id, vie_1, vie_2, vie_3</returns>
        [Route("addVieResult")]
        public HttpResponseMessage PostAddVieResult()
        {
            try
            {
                var id = System.Web.HttpContext.Current.Request.Params["image_id"];
                var vie1 = System.Web.HttpContext.Current.Request.Params["vie_1"];
                var vie2 = System.Web.HttpContext.Current.Request.Params["vie_2"];
                var vie3 = System.Web.HttpContext.Current.Request.Params["vie_3"];

                var image = from im in db.Images
                            where im.imageId == id
                            select im;
                if (image.Count() == 0)
                {
                    return ErrorResponseMessage("Hình ảnh không hợp lệ.");
                }
                var result = from im in db.VieResults
                             where im.vieId == id
                             select im;
                if (result.Count() == 0)
                {
                    VieResult newResult = new VieResult
                    {
                        vieId = id,
                        vie1 = vie1,
                        vie2 = vie2,
                        vie3 = vie3,
                        active = true
                    };
                    db.VieResults.InsertOnSubmit(newResult);
                    db.SubmitChanges();
                    ObjectResponse response = new ObjectResponse
                    {
                        status = new Status(200, "Thêm vào thành công"),
                        data = new Dictionary<string, dynamic>()
                    };
                    response.data.Add("vie_id", newResult.vieId);
                    response.data.Add("vie_1", newResult.vie1);
                    response.data.Add("vie_2", newResult.vie2);
                    response.data.Add("vie_3", newResult.vie3);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    result.First().vie1 = vie1;
                    result.First().vie2 = vie2;
                    result.First().vie3 = vie3;
                    result.First().active = true;
                    db.SubmitChanges();
                    ObjectResponse response = new ObjectResponse
                    {
                        status = new Status(200, "Ghi đè thành công"),
                        data = new Dictionary<string, dynamic>()
                    };
                    response.data.Add("vie_id", result.First().vieId);
                    response.data.Add("vie_1", result.First().vie1);
                    response.data.Add("vie_2", result.First().vie2);
                    response.data.Add("vie_3", result.First().vie3);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                return ErrorResponseMessage("Có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }

        /// <summary>
        /// Function: Remove an VieResult
        /// </summary>
        /// <returns>success or fail</returns>
        [Route("removeVieResult")]
        public HttpResponseMessage DeleteRemoveVieResult()
        {
            try
            {
                var id = System.Web.HttpContext.Current.Request.Params["vie_id"];
                var result = from im in db.VieResults
                             where im.vieId == id
                             select im;
                if (result.Count() == 0)
                {
                    return ErrorResponseMessage("Không tìm thấy kết quả.");
                }
                else
                {
                    result.First().active = false;
                    db.SubmitChanges();
                    ObjectResponse response = new ObjectResponse
                    {
                        status = new Status(200, "Xóa thành công"),
                        data = new Dictionary<string, dynamic>()
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, response); ;
                }
            }
            catch (Exception ex)
            {
                return ErrorResponseMessage("Có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }

        #endregion VieResult

        #region Core

        /// <summary>
        /// Function: get all result of one user.
        /// </summary>
        /// <returns>something</returns>
        [Route("getAllResultOfOneUser")]
        public HttpResponseMessage PostGetAllResultOfOneUser()
        {
            try
            {
                var username = System.Web.HttpContext.Current.Request.Params["username"];

                
                var user = from us in db.Users
                           where us.userName == username
                           select us;
                if (user.Count() == 0)
                {
                    return ErrorResponseMessage("Không tìm thấy người dùng");
                }
                var userId = user.First().userId;

                ObjectResponse response = new ObjectResponse
                {
                    status = new Status(200, "Lấy dữ liệu thành công"),
                    data = new Dictionary<string, dynamic>()
                };
                List<dynamic> list = new List<dynamic>();
                var imageList = from im in db.Images
                            where im.userId == userId && im.active == true
                            select im;
                foreach (Image image in imageList) {
                    Dictionary<String, dynamic> im = new Dictionary<string, dynamic>();
                    im.Add("image_id", image.imageId);
                    im.Add("image_name", image.imageName);
                    im.Add("time_shoot", image.timeShoot);
                    var engResult = from en in db.EngResults
                                    where en.engId == image.imageId && en.active == true
                                    select en;
                    if (engResult.Count() != 0)
                    {
                        Dictionary<String, dynamic> engSub = new Dictionary<string, dynamic>();
                        engSub.Add("eng_1", engResult.First().eng1);
                        engSub.Add("eng_2", engResult.First().eng2);
                        engSub.Add("eng_3", engResult.First().eng3);
                        im.Add("eng_sub", engSub);
                    }
                    var vieResult = from vi in db.VieResults
                                    where vi.vieId == image.imageId && vi.active == true
                                    select vi;
                    if (vieResult.Count() != 0)
                    {
                        Dictionary<String, dynamic> vieSub = new Dictionary<string, dynamic>();
                        vieSub.Add("vie_1", vieResult.First().vie1);
                        vieSub.Add("vie_2", vieResult.First().vie2);
                        vieSub.Add("vie_3", vieResult.First().vie3);
                        im.Add("vie_sub", vieSub);
                    }
                    list.Add(im);
                }
                response.data.Add("data", list);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponseMessage("Có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }
        
        #endregion Core

    }
}
