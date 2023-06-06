$uid = $.cookie('uid');
$token = $.cookie('token');
console.log("$uid:"+$uid+",token:"+$token);
let messages = {
  zh:{
    title:'登录Demo系统',
    titleMain:'Demo系统',
    contentMain:'欢迎登录Demo系统',
    logout:'登出',
    main:'主页面',
    home:'首页',
    tables:'表',
    charts:'图表',
    forms:'表格',
    exampleDropdown:'下拉示范',
    exampleDropdownPage1:'页面1',
    exampleDropdownPage2:'页面2',
    exampleDropdownPage3:'页面3',
    extras:'额外',
    ExtrasDemo1:'示范1',
    ExtrasDemo2:'示范2',
    ExtrasDemo3:'示范3'
  },
  en:{
    title:'Login demo system',
    titleMain:'Demo system',
    contentMain:'Welcome login demo system',
    logout:'Logout',
    main:'Main',
    home:'Home',
    tables:'Tables',
    charts:'Charts',
    forms:'Forms',
    exampleDropdown:'Example Dropdown',
    exampleDropdownPage1:'Page1',
    exampleDropdownPage2:'Page2',
    exampleDropdownPage3:'Page3',
    extras:'Extras',
    ExtrasDemo1:'Demo1',
    ExtrasDemo2:'Demo2',
    ExtrasDemo3:'Demo3'
  }
};
setLanguage(getLanguage());
if ($uid === undefined || $token === undefined)
{
  window.location.href="login.html";
}
else
{
  CheckTokenValid();
}
function GetMail()
{
  console.log('GetMail');
  $.getJSON("HttpGetMail",{},function(data,status){
    if (status === "success")
    {
      if (data.msg === '')//valid
      {
        console.log("GetMail:"+JSON.stringify(data));
        var mails = $("#mails");
        document.getElementById("mailCount").innerHTML = data.data.length;
        mails.empty();
        for(var i=0; i<data.data.length; i++)
        {
          var mail = data.data[i];
          mails.append('<li><a rel="nofollow" class="dropdown-item d-flex"><div class="msg-profile"><img src="img/avatar-1.jpg" alt="..." class="img-fluid rounded-circle"></div><div class="msg-body"><h3 class="h5">'
          +mail.title+
          '</h3><span>From '+mail.senderName+'</span></div></a></li>');
        }
        mails.append('<li><a rel="nofollow" href="#" class="dropdown-item all-notifications text-center"> <strong>Read all messages   </strong></a></li>');
      }
      else//expired
      {
        alert("HttpGetMail:"+data.msg);
      }
    }
    else
    {
      alert("HttpGetMail:"+data.msg);
    }
  });
}
function GetLog()
{
  $.getJSON("HttpGetLogAccountStat",{day:-9},function(data,status){
    if (status === "success")
    {
      if (data.msg === '')//valid
      {
        console.log("HttpGetLogAccountStat:"+JSON.stringify(data));
        var legendState = true;
        //if ($(window).outerWidth() < 576) {
          //legendState = false;
        //}
        var lineData = {
          type: 'line',
          options: {
            scales: {
              xAxes: [{
                display: true,
                gridLines: {
                  display: false
                }
              }],
              yAxes: [{
                display: true,
                gridLines: {
                  display: false
                }
              }]
            },
            legend: {
              display: legendState
            }
          },
          data: {
            //labels: ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17"],
            datasets: [
              {
                label: "login",
                fill: true,
                lineTension: 0,
                backgroundColor: "transparent",
                borderColor: '#f15765',
                pointBorderColor: '#da4c59',
                pointHoverBackgroundColor: '#da4c59',
                borderCapStyle: 'butt',
                borderDash: [],
                borderDashOffset: 0.0,
                borderJoinStyle: 'miter',
                borderWidth: 1,
                pointBackgroundColor: "#fff",
                pointBorderWidth: 1,
                pointHoverRadius: 5,
                pointHoverBorderColor: "#fff",
                pointHoverBorderWidth: 2,
                pointRadius: 1,
                pointHitRadius: 0,
                //data: [50, 20, 60, 31, 52, 22, 40, 25, 30, 68, 56, 40, 60, 43, 55, 39, 47],
                spanGaps: false
              },
              {
                label: "register",
                fill: true,
                lineTension: 0,
                backgroundColor: "transparent",
                borderColor: "#54e69d",
                pointHoverBackgroundColor: "#44c384",
                borderCapStyle: 'butt',
                borderDash: [],
                borderDashOffset: 0.0,
                borderJoinStyle: 'miter',
                borderWidth: 1,
                pointBorderColor: "#44c384",
                pointBackgroundColor: "#fff",
                pointBorderWidth: 1,
                pointHoverRadius: 5,
                pointHoverBorderColor: "#fff",
                pointHoverBorderWidth: 2,
                pointRadius: 1,
                pointHitRadius: 10,
                //data: [20, 7, 35, 17, 26, 8, 18, 10, 14, 46, 30, 30, 14, 28, 17, 25, 17, 40],
                spanGaps: false
              }
            ]
          }
        };
        lineData.data.labels = data.days;
        lineData.data.datasets[0].data = data.logins;
        lineData.data.datasets[1].data = data.registers;
        new Chart($('#lineChart'), lineData);
      }
      else//expired
      {
        alert("HttpGetLogAccountStat:"+data.msg);
      }
    }
    else
    {
      alert("HttpGetLogAccountStat:"+data.msg);
    }
  });
}

function CheckTokenValid()
{
  $.getJSON("HttpAccountToken",{},function(data,status){
    if (status === "success")
    {
      if (data.msg === '')//valid
      {
        console.log("HttpAccountToken:"+JSON.stringify(data));
        document.getElementById("nickname").innerText = data.nickname;
        document.getElementById("nickname2").innerText = data.nickname;
        setTimeout(GetMail, 0.5);
        setTimeout(GetLog, 0.5);
      }
      else//expired
      {
        $.removeCookie('uid', { expires: 1, path: '/' });
        $.removeCookie('token', { expires: 1, path: '/' });
        alert("HttpAccountToken:"+data.msg);
        window.location.href="login.html";
      }
    }
    else
    {
      alert("HttpAccountToken:"+data.msg);
    }
  });
}
function onClickLogout()
{
  $.getJSON("HttpAccountLogout",{},function(data,status){
    if (status === "success")
    {
      console.log("HttpAccountLogout:"+JSON.stringify(data));
      if (data.msg === '')
      {
        $.removeCookie('uid', { expires: 1, path: '/' });
        $.removeCookie('token', { expires: 1, path: '/' });
        window.location.href="login.html";
      }
      else
      {
        alert("HttpAccountLogout:"+data.msg);
      }
    }
    else
    {
      console.log("HttpAccountLogout:"+status);
    }
  });
}