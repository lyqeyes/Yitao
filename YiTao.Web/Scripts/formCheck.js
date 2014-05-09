

function whetherNull(id)
{
    var valueOfText = $("#"+id).val();
    if (valueOfText == "" || valueOfText == "不能为空") {
        $("#" + id).addClass("errorBorder").val("不能为空");
        return false;
    }
    else
        return true;
}

$(".my-form-control").focus(function () {
    $(this).removeClass("errorBorder");
    if ($(this).val() == "不能为空")
        $(this).val("");
});

$(".form-control").addClass("my-form-control");

$("*[name='description']").attr("rows", 6);