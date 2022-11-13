$(document).ready(function () {
	//responsive menu toggle
	$("#menutoggle").click(function () {
		$('.xs-menu').toggleClass('displaynone');

	});
	//add active class on menu
	/*$('li').click(function (e) {
		//e.preventDefault();
		$('li').removeClass('active');
		$(this).addClass('active');
	});*/
	if (document.baseURI.includes('Valeting')) {
		$('li').removeClass('active');
		$('#menuValeting').addClass('active');
	}
	else {
		$('li').removeClass('active');
		$('#menuHome').addClass('active');
	}
	
	//drop down menu	
	$(".drop-down").hover(function () {
		$('.mega-menu').addClass('display-on');
	});
	$(".drop-down").mouseleave(function () {
		$('.mega-menu').removeClass('display-on');
	});

});
