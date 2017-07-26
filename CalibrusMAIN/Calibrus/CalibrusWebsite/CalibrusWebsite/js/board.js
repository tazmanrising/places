$(function() {

	//expand content
	$('.toggle-bio').click( function (e) {
		
		//toggle expandible content
		if ($(this).hasClass("james")) {
			$('.james.full-bio').fadeToggle('slow');
		} 
		if ($(this).hasClass("hugh")) {
			$('.hugh.full-bio').fadeToggle('slow');
		} 
		if ($(this).hasClass("kent")) {
			$('.kent.full-bio').fadeToggle('slow');
		} 
		if ($(this).hasClass("nicholas")) {
			$('.nicholas.full-bio').fadeToggle('slow');
		} 
		if ($(this).hasClass("chris")) {
			$('.chris.full-bio').fadeToggle('slow');
		} 

		//change link text
		if ($(this).text() == 'show more...') {
			$(this).text('show less...');
		} else {
		$(this).text('show more...');
		}	
		return false;
	});
});