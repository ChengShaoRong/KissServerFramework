var randomStr = '';
function onClickValidCode()
{
  randomStr = randomString(32);
  setValidCode($("#validCode"), randomStr);
}
let messages = {
  zh:{
    title:'Demo系统首页',
    titleMain:'Demo系统',
    contentMain:'欢迎登录Demo系统',
    labelUser:'用户名',
    labelPW:'密码',
    labelVC:'验证码',
    labelForget:'忘记密码?',
    labelHave:'您是否尚未注册用户? ',
    labelSignup:'注册',
    labelAgree:'我已阅读且同意',
    labelAgree2:'用户隐私政策',
    policyTitle:'隐私政策',
    close:'关闭'
  },
  en:{
    title:'Demo system home',
    titleMain:'Demo system',
    contentMain:'Welcome login demo system',
    labelUser:'User Name',
    labelPW:'Password',
    labelVC:'Valid code',
    labelForget:'Forgot Password?',
    labelRemember:'Remember your account? ',
    login:'Login',
    labelSignup:'Signup',
    labelAgree:'I had read and agreed the ',
    labelAgree2:'terms and policy',
    policyTitle:'policy',
    close:'Close'
  }
};
setLanguage(getLanguage());
onClickValidCode();
function onClickLogin()
{
  if(!$("#login-username").valid()
      || !$("#login-password").valid()
      || !$("#register-agree").valid()
      || !$("#register-valid").valid())
  {
    console.log("username or password or code invalid or not agree policy");
    return;
  }
  $.post("HttpAccountLogin",{name:$('#login-username').val(),password:sha1($('#login-password').val()+'l]a(2;DK8+DLj-L3`/#sVpoM(b~Wk}7!'),token:randomStr,validCode:$('#register-valid').val()},function(data, status){
    enableA($('#login'), true);
    if (status === "success")
    {
      console.log("HttpAccountLogin:"+JSON.stringify(data));
      if (data.msg === '')
      {
        $.cookie('uid', data.uid, { expires: 1, path: '/' });
        $.cookie('token', data.token, { expires: 1, path: '/' });
        window.location.href="index.html";
      }
      else
      {
        onClickValidCode();
        showWarn(data.msg);
        //alert("HttpAccountLogin:"+data.msg);
      }
    }
    else
    {
      console.log("HttpAccountLogin:"+status);
      showError(status);
      //alert("HttpAccountLogin:"+status);
    }
  },"json");
  enableA($('#login'), false);
}