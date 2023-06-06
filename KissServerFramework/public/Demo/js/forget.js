
var randomStr = '';
function onClickValidCode()
{
  randomStr = randomString(32);
  setValidCode($("#validCode"), randomStr);
}
let messages = {
  zh:{
    title:'重设密码',
    titleMain:'重设密码',
    contentMain:'欢迎使用密保邮箱重设密码功能',
    labelEMail:'密保邮箱',
    labelVC:'验证码',
    labelRemember:'您是否已经记起账号密码了? ',
    login:'登录',
    forget:'发邮件'
  },
  en:{
    title:'Reset password',
    titleMain:'Reset password',
    contentMain:'Welcome to reset password by email',
    labelEMail:'Secure Email Address',
    labelVC:'Valid code',
    labelRemember:'Remember your account? ',
    login:'Login',
    forget:'Send email'
  }
};
setLanguage(getLanguage());
onClickValidCode();
function onClickForget()
{
  if(!$("#forget-email").valid()
      || !$("#forget-valid").valid())
  {
    console.log("email or code invalid");
    return;
  }
  $.post("HttpAccountRequestResetPassword",{email:$('#forget-email').val(),token:randomStr,validCode:$('#forget-valid').val(),lang:getLanguage()},function(data, status){
    enableA($('#forget'), true);
    if (status === "success")
    {
      console.log("HttpAccountRequestResetPassword:"+JSON.stringify(data));
      if (data.msg === '')
      {
        alert("Reset password web link had send to your email. Please check out your email and then modify your password.");
        window.location.href="login.html";
      }
      else
      {
        onClickValidCode();
        alert("HttpAccountLogin:"+data.msg);
      }
    }
    else
    {
      console.log("HttpAccountRequestResetPassword:"+status);
      alert("HttpAccountRequestResetPassword:"+status);
    }
  },"json");
  enableA($('#forget'), false);
}