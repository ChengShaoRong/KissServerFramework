
var params = parseUrl();
var time = 5;
let uid = 0;
let messages = {
  zh:{
    title:'确认密保邮箱',
    titleMain:'确认密保邮箱',
    contentMain:'欢迎使用密保邮箱重设密码功能',
    token:'缺少参数"token".',
    success:'确认密保邮箱成功. 将自动跳转到"index.html",在 {0} 秒后.',
    fail:'确认密保邮箱失败. 将自动跳转到"login.html",在 {0} 秒后.'
  },
  en:{
    title:'Confirm secure email',
    titleMain:'Confirm secure email',
    contentMain:'Welcome to confirm secure email',
    token:'Missing parameter "token".',
    success:'Confirm email success. Redirect to "index.html" in {0} seconds.',
    fail:'Confirm email fail. Redirect to "login.html" in {0} seconds.'
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
}
else
{
  $.post("HttpAccountConfirmEmail",{token:params.token},function(data, status){
    enableA($('#reset'), true);
    if (status === "success")
    {
      console.log("HttpAccountConfirmEmail:"+JSON.stringify(data));
      if (data.msg === '')
      {
        uid = $.cookie('uid');
        onCountDown();
        setInterval('onCountDown()', 1000);
      }
      else
      {
        document.getElementById("Tips").innerHTML = 'HttpAccountConfirmEmail : '+data.msg;
      }
    }
    else
    {
      document.getElementById("Tips").innerHTML = 'HttpAccountConfirmEmail : '+status;
    }
  },"json");
}
function onCountDown()
{
  if (uid > 0)
  {
    document.getElementById("Tips").innerHTML = (msg.success+'').format(time);
    if (time < 0)
      window.location.href="index.html";
  }
  else
  {
    document.getElementById("Tips").innerHTML = (msg.fail+'').format(time);
    if (time < 0)
      window.location.href="login.html";
  }
  time--;
}