function enableA(value, enable)
{
    if (enable)
    {
        value.removeClass("btn btn-primary disabled");
        value.addClass("btn btn-primary");
    }
    else
    {
        value.removeClass("btn btn-primary");
        value.addClass("btn btn-primary disabled");
    }
}
function setValidCode(obj, randomStr)
{
    obj.attr("src","GetValidCode?r="+randomStr);
}
function randomString(len, charSet)
{
    charSet = charSet || 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let randomString = '';
    for (let i = 0; i < len; i++) {
        let randomPoz = Math.floor(Math.random() * charSet.length);
        randomString += charSet.substring(randomPoz,randomPoz+1);
    }
    return randomString;
}
function showWarn(msg)
{
    showMsg(msg, 1);
}
function showInfo(msg)
{
    showMsg(msg, 0);
}
function showError(msg)
{
    showMsg(msg, 2);
}
function showMsg(msg, level = 0)
{
    let tip = $('#showTipsMsg');
    tip.removeClass('text-success text-warning text-danger')
    if (level === 0)
        tip.addClass('text-success');
    else if (level === 1)
        tip.addClass('text-warning');
    else
        tip.addClass('text-danger');
    document.getElementById("showTipsMsg").innerText = msg;
    $('#showTips').modal('show');
}

/**生成一个随机数字**/
function randomNum(min, max)  {
    return Math.random()*(max-min)+min;
}

/**生成一个随机色**/
function randomColor(min, max) {
    var r = randomNum(min, max);
    var g = randomNum(min, max);
    var b = randomNum(min, max);
    return "rgb(" + r + "," + g + "," + b + ")";
}
function parseUrl()
{
    var searchHref = window.location.search.replace('?', '');
    var params = searchHref.split('&');
    var returnParam = {};
    params.forEach(function(param)
    {
        var paramSplit = param.split('=');
        returnParam[paramSplit[0]] = paramSplit[1];
    });
    return returnParam;
}
function getCurrentPage()
{
    let curPage = $.cookie('page');
    if (curPage === undefined)
        curPage = 'home';
    return curPage;
}
function setCurrentPage(newPage)
{
    console.log("onClickPage:"+newPage);
    let old = getCurrentPage();
    if (old !== newPage)
        $('#'+old).parent().removeClass('active');
    $.cookie('page', newPage);
    $('#'+newPage).parent().addClass('active');
    setMainContent();
}
function setMainContent()
{
    let _modifyDay = document.getElementById("_modifyDay");
    if (_modifyDay == null)
        return;
    let curPage = getCurrentPage();
    let path = 'content/'+curPage+'_'+getLanguage()+'.html';
    let msg;
    if (getLanguage() === 'zh')
        msg = messages.zh.modifyDay;
    else
        msg = messages.en.modifyDay;
    if (curPage in txtVersion)
    {
        path = path + '?v=' + txtVersion[curPage];
        _modifyDay.innerText = msg.format(txtVersion[curPage]);
    }
    else
    {
        _modifyDay.innerText = msg.format(txtVersion.default);
    }
    $('#mainContent').load(path,function (responseTxt,statusTxt){
        console.log(statusTxt);
        if (statusTxt === 'success')
            dp.SyntaxHighlighter.HighlightAll('code');
    });
}
function getLanguage()
{
    let lang = $.cookie('lang');
    if (lang === undefined)
    {
        let browserLang = navigator.language || navigator.userLanguage;
        if (browserLang.substring(0,3) === 'zh-')
            lang = 'zh';
        else
            lang = 'en';
    }
    return lang;
}
function setLanguage(lang)
{
    $.cookie('lang', lang);
    let _modifyDay = document.getElementById("_modifyDay");
    let prefix = document.getElementById("_modifyDay") == null ? '' : 'Demo/';
    if (lang === 'zh')
    {
        $('#languageImg').attr('src', prefix+'img/flags/16/CN.png');
        document.getElementById("languageTxt").innerText = 'Change Language:简体中文';
    }
    else
    {
        $('#languageImg').attr('src', prefix+'img/flags/16/GB.png');
        document.getElementById("languageTxt").innerText = '修改语言:English';
    }
    let msg;
    if (lang === 'zh')
        msg = messages.zh;
    else
        msg = messages.en;
    for(let key in msg)
    {
        let elem = document.getElementById(key);
        if (elem !== null)
            elem.innerText = msg[key];
        else
            console.log('Not exist element '+key);
    }
    setMainContent();
}
function onClickPolicy()
{
    $("#policyContent").load("policy"+getLanguage()+".txt");
}
(function () {
    /// <summary>
    /// 引号转义符号
    /// </summary>
    String.EscapeChar = '\'';
    /// <summary>
    /// 替换所有字符串
    /// </summary>
    /// <param name="searchValue">检索值</param>
    /// <param name="replaceValue">替换值</param>
    String.prototype.replaceAll = function (searchValue, replaceValue) {
        var regExp = new RegExp(searchValue, "g");
        return this.replace(regExp, replaceValue);
    }
    /// <summary>
    /// 格式化字符串
    /// </summary>
    String.prototype.format = function () {
        var regexp = /\{(\d+)\}/g;
        var args = arguments;
        var result = this.replace(regexp, function (m, i, o, n) {
            return args[i];
        });
        return result.replaceAll('%', String.EscapeChar);
    }
})();