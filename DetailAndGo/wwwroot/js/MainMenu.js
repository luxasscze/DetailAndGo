$(document).ready(function () {
	//responsive menu toggle
	$("#menutoggle").click(function () {
		$('.xs-menu').toggleClass('displaynone');

	});	
	if (document.baseURI.includes('Valeting')) {
		$('li').removeClass('active');
		$('#menuValeting').addClass('active');
	}
	else if (document.baseURI.includes('Detailing')) {
		$('li').removeClass('active');
		$('#menuDetailing').addClass('active');
	}
	else if (document.baseURI.includes('Gallery')) {
		$('li').removeClass('active');
		$('#menuGallery').addClass('active');
	}
	else if (document.baseURI.includes('About')) {
		$('li').removeClass('active');
		$('#menuAbout').addClass('active');
	}
	else if (document.baseURI.includes('Contact')) {
		$('li').removeClass('active');
		$('#menuContact').addClass('active');
	}
	else if (document.baseURI.includes('MyAccount')) {
		$('li').removeClass('active');
		$('#menuMyAccount').addClass('active');
	}
	else if (document.baseURI.includes('MyCar')) {
		$('li').removeClass('active');
		$('#menuMyCar').addClass('active');
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
