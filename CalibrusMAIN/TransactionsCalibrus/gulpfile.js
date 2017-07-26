var gulp = require('gulp');
var jshint = require('gulp-jshint');
var jscs = require('gulp-jscs');
var nodemon = require('gulp-nodemon');

var jsFiles = ['*.js', 'public/**/*.js'];

//create a task.


gulp.task('style', function(){
   return gulp.src(jsFiles)  //return the string for a subtask
        .pipe(jshint())
        .pipe(jshint.reporter('jshint-stylish',{
            verbose: true
        }))
        .pipe(jscs());
});

//  gulp style

//npm install --save-dev jshint gulp-jshint

// npm install wiredep --save-dev

gulp.task('inject', function(){
    var wiredep = require('wiredep').stream;
    var inject = require('gulp-inject');

    // use array [] since need both css and js locations
    var injectSrc = gulp.src(
        ['./public/css/*.css',
        './public/js/*.js',
        './public/env.js',
        './public/app.js',
        './public/services/*.js',
        './public/controllers/*.js',
        './public/directives/*.js'],
        {read: false});
    var injectOptions = {
        ignorePath: '/public',
        addSuffix: '?v=' + new Date().getTime() //'?ver=1' //timeStamp
    };

    var options = {
        bowerJson: require('./bower.json'),
        directory: './public/lib',
        ignorePath: '../../public'
    };


    // wiredep looks at our bower file
    // return gulp.src('./src/views/*.html')
    //     .pipe(wiredep(options))
    //     .pipe(inject(injectSrc, injectOptions))
    //     .pipe(gulp.dest('./src/views'));
    return gulp.src('./public/*.html')
        .pipe(wiredep(options))
        .pipe(inject(injectSrc, injectOptions))
        .pipe(gulp.dest('./public'));

});

// run this

// gulp inject


// npm install gulp-inject --save-dev

// npm install --save-dev gulp-nodemon

//   lets run  gulp serve   and it runs style and inject
//gulp.task('serve', ['style', 'inject'], function() {
gulp.task('serve',['inject'], function() {
    var options = {
        script: './bin/www',
        delayTime: 1,
        env: {
            'PORT': 4500
        },
        watch: jsFiles
    }

    return nodemon(options)
        .on('restart', function(ev){
            console.log('Restarting...');
        })
});