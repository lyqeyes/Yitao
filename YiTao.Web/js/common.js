jQuery(function($){
	var WindowHeight = $(window).height();
	$('.warp').css('height',WindowHeight +'px');
	$(window).ready(function(e) {
		$('.flexslider').flexslider({
			animation : 'slide',
			easing: "easeInOutExpo",
			animationLoop:!0
		}); 
    });
	/***************积分兑换顶部按钮的效果切换****************/
	$('.integral_title div:nth-child(1)').click(function(){
		if($(this).hasClass('title_left'))return;
		else {	
			$(this).removeClass('title_left01').addClass('title_left');
			$('.integral_title div:nth-child(2)').removeClass('title_right').addClass('title_right01');
		}
	});
	$('.integral_title div:nth-child(2)').click(function(){
		if($(this).hasClass('title_right'))return;
		else {	
			$(this).removeClass('title_right01').addClass('title_right');
			$('.integral_title div:nth-child(1)').removeClass('title_left').addClass('title_left01');
		}
	});
	/*********************	商品列表的筛选按钮****************************/
	$('.filter li').click(function(){
		for(var i = 0; i < $('.filter li').length; i ++)
		{
			$('.filter li').eq(i).hasClass('active') && $('.filter li').eq(i).removeClass('active');
		}
		$(this).addClass('active');
	});
});

