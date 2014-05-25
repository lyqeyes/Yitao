$('.ToCollect').click(function () {
    var index = $('.ToCollect').index(this);
    var accountId = $(this).attr('data-account');
    var shangpinId = $(this).attr('data-ShangpinId');
    var type = $(this).attr('data-type');
    var that = this;
    $.ajax({
        url: "/Watch/Collection/Collect",
        method: "POST",
        data: {
            AccountId: accountId,
            ShangpinId: shangpinId,
            type: type
        },
        success: function (res) {
            if (res == "ok") {
                $(that).addClass('hide');
                $('.Anti-Collect').eq(index).removeClass('hide');
                if ($('.ToCollect').eq(index).hasClass('collect-img')) {
                    $('.ToCollect').eq(index + 1).addClass('hide');
                    $('.Anti-Collect').eq(index + 1).removeClass('hide');
                }
                else {
                    $('.ToCollect').eq(index - 1).addClass('hide');
                    $('.Anti-Collect').eq(index - 1).removeClass('hide');
                }    
            }
        },
        error : function () {
            alert("不好意思，出现错误，收藏失败");
        }
    });
});
$('.Anti-Collect').click(function () {
    var index = $('.Anti-Collect').index(this);
    var accountId = $(this).attr('data-account');
    var shangpinId = $(this).attr('data-ShangpinId');
    var type = $(this).attr('data-type');
    var that = this;
    $.ajax({
        url: "/Watch/Collection/anti_Collect",
        method: "POST",
        data: {
            AccountId: accountId,
            ShangpinId: shangpinId,
            type: type
        },
        success: function (res) {
            if (res == "ok") {
                $(that).addClass('hide');
                $('.ToCollect').eq(index).removeClass('hide');
                if ($(that).hasClass('collect-img')) {
                    $('.Anti-Collect').eq(index + 1).addClass('hide');
                    $('.ToCollect').eq(index + 1).removeClass('hide');
                }
                else {
                    $('.Anti-Collect').eq(index - 1).addClass('hide');
                    $('.ToCollect').eq(index - 1).removeClass('hide');
                }
            }
            else if (res == "error") {
                alert("不好意思，出现错误，取消收藏失败");
            }
        },
        error: function () {
            alert("不好意思，出现错误，取消收藏失败");
        }
    });
});
$('.List-Anti-Collect').click(function () {
    var accountId = $(this).attr('data-account');
    var shangpinId = $(this).attr('data-ShangpinId');
    var type = $(this).attr('data-type');
    var that = this;
    $.ajax({
        url: "/Watch/Collection/anti_Collect",
        method: "POST",
        data: {
            AccountId: accountId,
            ShangpinId: shangpinId,
            type: type
        },
        success: function (res) {
            if (res == "ok") {
                $(that).parent().parent().parent().hide();
            }
            else if (res == "error") {
                alert("不好意思，出现错误，收藏失败");
            }
        },
        error: function () {
            alert("不好意思，出现错误，收藏失败");
        }
    });
});