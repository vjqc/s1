using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using BiblioContenidos_2.Models;

using CaptchaMVC.HtmlHelpers;

namespace BiblioContenidos_2.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn
        

        [HttpPost]
        //public ActionResult LogOn(CaptchaModel cm, LogOnModel model, string returnUrl)
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    if (Session["captcha"] != null && (int)Session["captcha"] > 2 && !this.IsCaptchaVerify("Captcha no válido"))
                    {
                        return View(model);
                    }

                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {

                        //Session["captcha"] = 0;
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    
                    ModelState.AddModelError("", "El nombre de usuario o la contraseña es incorrecto.");

                    if (Session["captcha"] != null && (int)Session["captcha"] > 2 && !this.IsCaptchaVerify("Captcha no válido"))
                    {
                        return View(model);
                    }
                    
                    if (Session["captcha"] == null)
                    {
                        Session["captcha"] = 0;
                    }
                    int c = (int)Session["captcha"];
                    c++;
                    Session["captcha"] = c;
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            Session.Clear();
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    DataClasses1DataContext db = new DataClasses1DataContext();
                    System.Guid IdUs = db.aspnet_Users.Where(a => a.UserName == model.UserName).Select(a => a.UserId).ToArray()[0];
                    System.Guid IdRol = db.aspnet_Roles.Where(a => a.RoleName == "Usuario").Select(a => a.RoleId).ToArray()[0];
                    aspnet_UsersInRole rel = new aspnet_UsersInRole() { RoleId = IdRol, UserId = IdUs };
                    db.aspnet_UsersInRoles.InsertOnSubmit(rel);

                    string email = db.aspnet_Memberships.Where(a => a.UserId == IdUs).Select(a => a.Email).ToArray()[0];
                    string pwd = db.aspnet_Memberships.Where(a => a.UserId == IdUs).Select(a => a.Password).ToArray()[0];
                    string nick = db.aspnet_Users.Where(a => a.UserId == IdUs).Select(a => a.UserName).ToArray()[0];
                    Usuario NuevoUsuario = new Usuario()
                    {
                        UserId = IdUs,
                        //Nick = nick,
                        //Password = pwd,
                        //Email = email,
                        Karma = 0
                    };
                    db.Usuarios.InsertOnSubmit(NuevoUsuario);
                    db.SubmitChanges();
                    

                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "El Nombre de Usuario ya existe. Por favor introduzca un nombre de usuario diferente.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Ese E-Mail ya esta registrado. Por favor introduzca un E-Mail diferente.";

                case MembershipCreateStatus.InvalidPassword:
                    return "La contraseña es inválida. Por favor introduzca una contraseña válida.";

                case MembershipCreateStatus.InvalidEmail:
                    return "El E-Mail no es válido. Por favor verifique e intente de nuevo.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "El Nombre de Usuario no es válido. Por favor verifique e intente de nuevo.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "Ocurrió un error desconocido. Por favor verifique e intente de nuevo.. Si el problema persiste, por favor contacte con el administrador de sistemas.";
            }
        }
        #endregion
    }
}
