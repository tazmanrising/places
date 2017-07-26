var gulp = require('gulp');
var jshint = require('gulp-jshint');
var jscs = require('gulp-jscs');
var nodemon = require('gulp-nodemon');

var jsFiles = ['*.js', 'src/**/*.js'];

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

gulp.task('inject', function () {
    var timestamp = new Date().getTime();
    var wiredep = require('wiredep').stream;
    var inject = require('gulp-inject');

    // use array [] since need both css and js locations
    var injectSrc = gulp.src(
        ['./public/css/*.css', 
        './public/js/*.js',
        'app.js',
        './src/services/*.js',
        './src/controllers/*.js'
        ], 
        {read: false});
    var injectOptions = {
        //ignorePath: '/public'
        addSuffix: '?v=' + new Date().getTime() //'?ver=1' //timeStamp
    };

    var options = {
        bowerJson: require('./bower.json'),
        directory: './public/lib'//,
        //ignorePath: 'public'
        //ignorePath: '../../public'
    };





     //wiredep looks at our bower file
     //return gulp.src('./src/views/*.html')
     //    .pipe(wiredep(options))
     //    .pipe(inject(injectSrc, injectOptions))
     //    .pipe(gulp.dest('./src/views'));

    return gulp.src('*.html')
      .pipe(wiredep(options))
      .pipe(inject(injectSrc, injectOptions))
      .pipe(gulp.dest(''));

     //return gulp.src('../CalibrusTPV.Scripts/index.html')
     //   .pipe(wiredep(options))
     //   .pipe(inject(injectSrc, injectOptions))
     //   .pipe(gulp.dest('../CalibrusTPV.Scripts/index.html'));
    
});

// run this

// gulp inject


// npm install gulp-inject --save-dev

// npm install --save-dev gulp-nodemon

//   lets run  gulp serve   and it runs style and inject
gulp.task('serve', ['style', 'inject'], function() {
    var options = {
        script: 'app.js',
        delayTime: 1,
        env: {
            'PORT': 3000
        },
        watch: jsFiles
    };

    return nodemon(options)
        .on('restart', function(ev){
            console.log('Restarting...');
        });
});



// This will copy the public/css/styles.css 
// and create 3 directories in apps called out1 etc with copy of that same file

// gulp stylecopy
gulp.task('stylecopy', function () {
    var dests = ['./out1', './out2', './out3'];
    var stream = gulp.src('./public/css/styles.css');
    for (var i = 0; i < dests.length; i++) {
        stream = stream.pipe(gulp.dest(dests[i]));
    }
    return stream;
});



gulp.task('default', function () {
    var dests = ['../out1.test'];
    var stream = gulp.src('./public/css/styles.css');
    for (var i = 0; i < dests.length; i++) {
        stream = stream.pipe(gulp.dest(dests[i]));
    }
    return stream;
});