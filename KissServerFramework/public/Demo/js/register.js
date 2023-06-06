
var randomStr = '';
function onClickValidCode()
{
  randomStr = randomString(32);
  setValidCode($("#validCode"), randomStr);
}
let messages = {
  zh:{
    title:'注册Demo系统',
    titleMain:'Demo系统',
    contentMain:'欢迎注册Demo系统',
    labelUser:'用户名',
    labelEMail:'密保邮箱',
    labelPW:'密码',
    labelPW2:'确认密码',
    labelVC:'验证码',
    labelAgree:'我已阅读且同意',
    labelAgree2:'用户隐私政策',
    register:'注册',
    login:'登录',
    loginTips:'是否已经有账号了?',
    policyTitle:'隐私政策',
    close:'关闭'
  },
  en:{
    title:'Register demo system',
    titleMain:'Demo system',
    contentMain:'Welcome register demo system',
    labelUser:'User Name',
    labelEMail:'Secure Email Address',
    labelPW:'Password',
    labelPW2:'Confirm password',
    labelVC:'Valid code',
    labelAgree:'I had read and agreed the ',
    labelAgree2:'terms and policy',
    register:'Register',
    login:'Login',
    loginTips:'Already have an account?',
    policyTitle:'policy',
    close:'Close'
  }
};
setLanguage(getLanguage());
onClickValidCode();
function onClickRegister()
{
  if(!$("#register-username").valid()
      || !$("#register-email").valid()
      || !$("#register-password").valid()
      || !$("#register-password2").valid()
      || !$("#register-agree").valid()
      || !$("#register-valid").valid())
  {
    console.log("input invalid");
    return;
  }
  $.post("HttpAccountRegister",{name:$('#register-username').val(),password:sha1($('#register-password').val()+'l]a(2;DK8+DLj-L3`/#sVpoM(b~Wk}7!'),email:$('#register-email').val(),token:randomStr,validCode:$('#register-valid').val()},function(data,status){
    if (status === "success")
    {
      console.log("HttpAccountRegister:"+JSON.stringify(data));
      if (data.msg === '')
      {
        $.cookie('uid', data.uid, { expires: 1, path: '/' });
        $.cookie('token', data.token, { expires: 1, path: '/' });
        window.location.href="index.html";
      }
      else
      {
        onClickValidCode();
        alert("HttpAccountRegister:"+data.msg);
      }
    }
    else
    {
      console.log("HttpAccountRegister:"+status);
      alert("HttpAccountRegister:"+status);
    }
  },"json");
}