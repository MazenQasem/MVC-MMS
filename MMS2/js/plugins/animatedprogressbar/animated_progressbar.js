$(document).ready(function(){
    jQuery.fn.anim_progressbar = function (aOptions) {
                 var iCms = 1000;
        var iMms = 60 * iCms;
        var iHms = 3600 * iCms;
        var iDms = 24 * 3600 * iCms;

                 var aDefOpts = {
            start: new Date(),              finish: new Date().setTime(new Date().getTime() + 60 * iCms),              interval: 100
        }
        var aOpts = jQuery.extend(aDefOpts, aOptions);
        var vPb = this;

                 return this.each(
            function() {
                var iDuration = aOpts.finish - aOpts.start;

                                 $(vPb).children('.pbar').progressbar();

                                 var vInterval = setInterval(
                    function(){
                        var iLeftMs = aOpts.finish - new Date();                          var iElapsedMs = new Date() - aOpts.start,                              iDays = parseInt(iLeftMs / iDms),                              iHours = parseInt((iLeftMs - (iDays * iDms)) / iHms),                              iMin = parseInt((iLeftMs - (iDays * iDms) - (iHours * iHms)) / iMms),                              iSec = parseInt((iLeftMs - (iDays * iDms) - (iMin * iMms) - (iHours * iHms)) / iCms),                              iPerc = (iElapsedMs > 0) ? iElapsedMs / iDuration * 100 : 0;  
                                                 $(vPb).children('.percent').html('<b>'+iPerc.toFixed(1)+'%</b>');
                        $(vPb).children('.elapsed').html(iDays+' days '+iHours+'h:'+iMin+'m:'+iSec+'s</b>');
                        $(vPb).children('.pbar').children('.ui-progressbar-value').css('width', iPerc+'%');

                                                 if (iPerc >= 100) {
                            clearInterval(vInterval);
                            $(vPb).children('.percent').html('<b>100%</b>');
                            $(vPb).children('.elapsed').html('Finished');
                            notify('Progressbar','Finished #'+$(vPb).attr('id'));
                        }
                    } ,aOpts.interval
                );
            }
        );
    }   
    
});