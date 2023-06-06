var params = parseUrl();
let messages = {
  zh:{
    title:'重设密码',
    titleMain:'重设密码',
    contentMain:'欢迎使用密保邮箱重设密码功能',
    labelPW:'密码',
    labelPW2:'确认密码',
    reset:'确认修改',
    token:'缺少参数"token".',
    name:'缺少参数"name".',
    login:'您的登录名 : '
  },
  en:{
    title:'Reset password',
    titleMain:'Reset password',
    contentMain:'Welcome to reset password by email',
    labelPW:'Password',
    labelPW2:'Confirm password',
    reset:'Confirm',
    token:'Missing parameter "token".',
    name:'Missing parameter "name".',
    login:'Your login name : '
  }
};
let lang = getLanguage();
let msg;
if (lang === 'zh')
  msg = messages.zh;
else
  msg = messages.en;
setLanguage(lang);
if (!params.hasOwnProperty('token'))
{
  document.getElementById("Tips").innerHTML = msg.token;
  $('#reset-form').hide();
}
else if (!params.hasOwnProperty('name'))
{
  document.getElementById("Tips").innerHTML = msg.name;
  $('#reset-form').hide();
}
else
{
  document.getElementById("loginName").innerHTML = msg.login+params.name;
}
function onClickResetPassword()
{
  if(!$("#reset-password").valid()
      || !$("#reset-password2").valid())
  {
    console.log("password invalid");
    return;
  }
  $.post("HttpAccountConfirmResetPassword",{passwordNew:sha1($('#reset-password').val()+'l]a(2;DK8+DLj-L3`/#sVpoM(b~Wk}7!'),token:params.token},function(data, status){
    enableA($('#reset'), true);
    if (status === "success")
    {
      console.log("HttpAccountRequestResetPassword:"+JSON.stringify(data));
      if (data.msg === '')
      {
        $.cookie('uid', data.uid, { expires: 1, path: '/' });
        $.cookie('token', data.token, { expires: 1, path: '/' });
        window.location.href="index.html";
      }
      else
      {
        alert("HttpAccountRequestResetPassword:"+data.msg);
      }
    }
    else
    {
      console.log("HttpAccountRequestResetPassword:"+status);
      alert("HttpAccountRequestResetPassword:"+status);
    }
  },"json");
  enableA($('#reset'), false);
}