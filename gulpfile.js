var gulp 		= require('gulp');

_website_dest_bin = '../../lnrs/sandbox/Website/bin';
_website_dest_cfg = '../../lnrs/sandbox/Website/app_config/include/modules';

_sourcedll = 'Sitecore.Modules.HtmlCacheBuilder/bin/Debug/*.dll';
_sourcepdb = 'Sitecore.Modules.HtmlCacheBuilder/bin/debug/*.pdb';
_sourcecfg = 'Sitecore.Modules.HtmlCacheBuilder/app_config/include/modules/*.config';



gulp.task('default', function(){

	gulp.watch([_sourcedll], function() {
		console.log(_website_dest_bin);
		gulp.src(_sourcedll)
			.pipe(gulp.dest(_website_dest_bin));
	});
	
	gulp.watch([_sourcepdb], function() {
		gulp.src(_sourcepdb)
			.pipe(gulp.dest(_website_dest_bin));
	});
	
	gulp.watch([_sourcecfg], function() {
		console.log(_website_dest_cfg);
		gulp.src(_sourcecfg)
			.pipe(gulp.dest(_website_dest_cfg));
	});

});
