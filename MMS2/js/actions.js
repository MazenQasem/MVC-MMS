$(document).ready(function(){
    
    
    $('html').click(function(){               
       $(".head .buttons > li").removeClass('active');
    });    
    
    
    
    
        var background = $.cookies.get('background');
        var theme      = $.cookies.get('theme');
        
        if(null != background)
            $("body").addClass(background);
        if(null != theme)
            $("body").addClass(theme);
        
    
    
    
    $(".backgrounds a").click(function(){        
        $('body').removeClass('bg_default bg_mgrid bg_crosshatch bg_yellow_hatch bg_hatch bg_green_hatch bg_texture bg_light_gray bg_dark_gray bg_light_orange').addClass($(this).attr('class'));
        $.cookies.set('background',$(this).attr('class')); 
        return false;
    });
    
    
    
    $(".themes a").click(function(){        
        $('body').removeClass('ssLight ssDark ssGreen ssRed ssTq ssGy ssDaB').addClass($(this).attr('data-theme'));
        $.cookies.set('theme',$(this).attr('data-theme')); 
        return false;
    });    
    
        
    $(".fmenu > li > a").click(function(){
        var ul = $(this).parent('li');
        if(ul.find('ul').length > 0){
            if(ul.hasClass('active')){
               ul.removeClass('active');
            }else{
               ul.addClass('active'); 
            }            
            return false;
        }        
    });    
    
    
    $("#sendMailModal").dialog({autoOpen: false, 
                                modal: true,
                                width: 600,
                                open: function(){                                    
                                    if($("#mail_wysiwyg").length > 0) m_editor.refresh();
                                    fix();
                                }});
    
    $("#openMailModal").click(function(){        
        $("#sendMailModal").dialog('open');
    });
    
    
    
        
    $(".sGallery .item").hoverIntent(function(){
        if($(this).find('.controls').length > 0){            
            $(this).find('.controls').animate({right: 0},'200');
        }
    },function(){
        if($(this).find('.controls').length > 0){            
            $(this).find('.controls').animate({right: -150},'200');
        }            
    });
    
    
    
                 $("div[data-collapse]").each(function(){
            
            var state = $.cookies.get('collapse_'+$(this).attr('data-collapse'));
            
            if(null != state && state == 'closed')
                $(this).hide();                
            
        });
        
                 $(".cblock").click(function(){
            var block = $(this).parents('.widget').find("[class^=block]");
            if(block.is(':visible')){
                
                block.fadeOut();
                if(null != block.attr('data-collapse'))
                    $.cookies.set('collapse_'+block.attr('data-collapse'),'closed');
            }else{
                
                block.fadeIn();
                if(null != block.attr('data-collapse'))
                    $.cookies.set('collapse_'+block.attr('data-collapse'),'opened');
            }
            
            return false;
        });
    
    
    
    $(".rating a.active").each(function(){
        $(this).attr('data-active','1');
    });
    $(".rating a").live('click',function(){
        $(this).addClass('active').prevAll().addClass('active').attr('data-active','1');
        $(this).attr('data-active','1').nextAll().removeClass('active').removeAttr('data-active');
        
        notify('Rating','Rate #'+$(this).parent('.rating').attr('id')+': '+$(this).attr('data-original-title'));
        return false;
    });

    $(".rating a").live({
        mouseenter: function(){
            $(this).addClass('active').prevAll().addClass('active');
            $(this).nextAll().removeClass('active');
        },
        mouseleave: function(){
            $(this).parent('div').find('a').removeClass('active');
            $(this).parent('div').find('a[data-active=1]').addClass('active');
        }        
    });
    
    
    
    
    
    $(".head .buttons > li > a").click(function(){        
        var li = $(this).parent('li');        
        if(li.find('ul').length > 0){
            if(li.hasClass('active'))
                li.removeClass('active');
            else
                li.addClass('active');
            return false;            
        }
        event.stopPropagation();
    });
    
    
    
    
    
    $(".alert").click(function(){
        $(this).animate({opacity: 0},'200','linear',function(){
            $(this).remove();
        });
    });
    
    
    
    
    $("table .checkall").click(function(){
        
        var iC = $(this).parents('th').index();          var tB = $(this).parents('table').find('tbody');          
        if($(this).is(':checked'))
            tB.find('tr').each(function(){                
                $(this).addClass('active').find('td:eq('+iC+') input:checkbox').attr('checked',true).parent('span').addClass('checked');
            });
        else
            tB.find('tr').each(function(){
                $(this).removeClass('active').find('td:eq('+iC+') input:checkbox').attr('checked',false).parent('span').removeClass('checked');
            });            
        
    });
    

    
    $(".file .btn, .file input:text").click(function(){        
        var block = $(this).parent('.file');
        block.find('input:file').click();
        block.find('input:file').change(function(){
            block.find('input:text').val(block.find('input:file').val());
        });
    });
            
    
    $("table .checker").click(function(event){
        
        var tr = $(this).parents('tr');
        
        if(tr.hasClass('active'))
            tr.removeClass('active');
        else
            tr.addClass('active');       
       
       event.stopPropagation();
    });
    
    
    $(".table-row-check tbody tr").click(function(){
        
       if($(this).hasClass('active'))
            $(this).removeClass('active');
        else
            $(this).addClass('active');
        
        $(this).find('input:checkbox').each(function(){
            
            if($(this).is(':checked')){
                $(this).attr('checked',false).parent('span').removeClass('checked');
            }else{
                $(this).attr('checked',true).parent('span').addClass('checked');
            }
                            
        });
        
    });

    

    var subNav = $.cookies.get('submainNavigation');
    var navigation = $.cookies.get('navigation');
    
    var subNavigation = '';
        if(null != navigation){
            subNavigation = navigation;
        }else
            subNavigation = 'fixed';
    
    var bw = $("body").width();
    if(bw <= 1152 && bw > 320){        
        subNavigation = 'collapsible';    
    }else if(bw <= 320){
        subNavigation = 'hidden';
    }else{
        if(null != subNav){
            if(subNav != 'hide' && subNavigation == 'collapsible')
                show_submainNav(false);        
        }
    }

    show_submainBlock();
        
    if(subNavigation == 'fixed'){        
        $("#fixedNav").addClass('active').parent('span').addClass('checked');        
        $("body").addClass('smf');
    }
    if(subNavigation == 'hidden'){        
        $("#hiddenNav").addClass('active').parent('span').addClass('checked');        
        $("body").addClass('smw');
    }    
    if(subNavigation == 'collapsible'){    
        $("#collapsedNav").addClass('active').parent('span').addClass('checked');
    }
        
        $(".navigation .main li a").click(function(){       

            if($(this).hasClass('active') && subNavigation != 'fixed' && $('.navigation .control').hasClass('active')){
                hide_submainNav(false);
                $(this).removeClass('active');                        
                return false;                        
            }

            var submain = $(this).attr('href');

            if($(submain).length > 0){
                $(".navigation .main li a").removeClass('active');
                $(".navigation .submain > div").hide();

                $(this).addClass('active');

                mnActive = $(this).attr('href');
                if($(mnActive).length > 0)
                    $(mnActive).fadeIn('300');
                else
                    $("#default").fadeIn('300');
                                               
                if(subNavigation != 'fixed') show_submainNav(false);

                return false;
            }
            
        });     

        $(".navigation .control").click(function(){
            if($(this).hasClass('active')){           
                hide_submainNav(true);                                    
            }else{                        
                show_submainBlock();            
                show_submainNav(true);                        
            }        
        });  
        
    $("#fixedNav").click(function(){
        if(!$(this).hasClass('active')){        
            $("#collapsedNav, #hiddenNav").removeClass('active');
            $(this).addClass('active');
            $("body").removeClass('smw').addClass('smf');
            $.cookies.set('navigation','fixed');
            subNavigation = 'fixed';                       
            fix();
            
        }
    });
    
    $("#collapsedNav").click(function(){
        if(!$(this).hasClass('active')){        
            $("#fixedNav, #hiddenNav").removeClass('active');
            $(this).addClass('active');
            $("body").removeClass('smf smw');
            $.cookies.set('navigation','collapsible');
            subNavigation = 'collapsible';
            fix();            
        }        
    });    
    
    $("#hiddenNav").click(function(){
        if(!$(this).hasClass('active')){        
            $("#fixedNav, #collapsedNav").removeClass('active');
            $(this).addClass('active');
            $("body").removeClass('smf').addClass('smw');
            $.cookies.set('navigation','hidden');
            subNavigation = 'hidden';
            fix();            
        }        
    });            
    
    
    
    $(".dropdown .label").click(function(){
                
        var dropdown = $(this).parent('.dropdown');
        
        if(dropdown.hasClass('active'))
            $(".header .buttons > div").removeClass('active');
        else{
            $(".header .buttons > div").removeClass('active');
            dropdown.addClass('active');
        }
             
    });
        

    
    $(".popup .label").click(function(){                                               
        var popup = $(this).parent('.popup');               
        if(popup.hasClass('active'))
            $(".header .buttons > div").removeClass('active');
        else{
            $(".header .buttons > div").removeClass('active');
            popup.addClass('active');        
        }
                
        $(".buttons .radio").show();        
        fix();        
    });
    
        

    $("#subNavControll").click(function(){
        if($('.navigation').hasClass('active'))
            $('.navigation').removeClass('active');
        else
            $('.navigation').addClass('active');
    });

    
    
    $("ul.list li .remove").click(function(){
        
        var item = $(this).parent('.actions').parent('li');
        
        notify('Remove','Item: '+item.attr('id'));
        
        item.fadeOut('300',function(){
            $(this).remove();
        });
        
        return false;
        
    });
    
   
    
    
    $(".sGallery .remove").click(function(){

        var item = $(this).parents('.item');

        notify('Remove','Gallery Item: '+item.attr('id'));

        item.fadeOut('300',function(){
            $(this).remove();
        });

        return false;

    });     
    
    
    
    
    $("#icoGly li").click(function(){
        $('#icoGly li').removeClass('active');
        $(this).addClass('active');
        $("#genGly").html('<code> &lt;span class="'+$(this).find('i').attr('class')+'"&gt;&lt;/span&gt;</code>');        
    });
    
    $("#icoMoon li").click(function(){
        $("#icoMoon li").removeClass('active');
        $(this).addClass('active');
        var cl = $(this).find('i').attr('class').split('-');
        
        $("#genIco").html('<code> &lt;span class="ico'+$('#getIconSize').val()+$("#getIconColor").val()+'-'+cl[1]+'"&gt;&lt;/span&gt;</code>');
    });
    
    
    middleNavigation();

});

$(window).load(function(){    
    fix();    
});

$(window).resize(function(){
    fix();    
    
    if($("body").width() <= 1152 && $("body").width() > 320){
        subNavigation = 'collapsible'; 
        $("body").removeClass('smf smw');
    }
    if($("body").width() <= 320){        
        subNavigation = 'hidden';
        $("body").removeClass('smf').addClass('smw');
    }
});

function fix(){
    
        
    fix_block_items_width('.input-prepend',['.add-on','button'],'input');
    fix_block_items_width('.input-append',['.add-on','button'],'input');    
        
    

    
    if($(".sGallery").length > 0)
        gallery_block_items_width('.sGallery','.item',10);
    
    if($(".Gallery").length > 0)
        gallery_block_items_width('.Gallery','.view',20);
    
    
    $(".checker").show();
    
    if($('#main_calendar').length > 0)
        $('#main_calendar').fullCalendar('render');
        
}

function fix_block_items_width(block,what,to){
    
    $(block).each(function(){
        
        var iWidth = $(this).width();
        
        if(what.length > 0){
            
            for(var i=0; i < what.length; i++){
                $(this).find(what[i]).each(function(){
                    iWidth -= $(this).width()+(parseInt($(this).css('padding-left')) * 2);
                });
            }
            $(this).find(to).width(iWidth-14);
            
        }
    });    
    
}

function gallery_block_items_width(block,item,plus){
    
    var bW = $(block).width();
    var iW = $(block).find(item).width()+plus;
    
    var iC = Math.floor(bW/iW);
    
    var iM = Math.floor( (bW-iW*iC)/(iC*2) );    
    $(block).find(item).css('margin-left',iM).css('margin-right',iM);
}

function show_submainBlock(){
    var sub = $(".navigation .main a.active").attr('href');
    
    if(sub.search("#") < 0){
        $("#default").show();        
    }else{    
        if($(sub).length > 0)
            $(sub).show();                        
        else
            $("#default").show();            
    }
    
}

function show_submainNav(rem){
    $(".navigation .control").addClass('active');            
    $(".navigation .submain").css('width', 220);
    
    if(rem) $.cookies.set('submainNavigation','show');    
    fix();
}

function hide_submainNav(rem){
    $(".navigation .control").removeClass('active');
    $(".navigation .submain").css('width',0);   
    
    if(rem) $.cookies.set('submainNavigation','hide');
}

function middleNavigation(){            
    
    var lock = false;
    
    var subs = new Array();
    subs[1] = {'left': '-50px', 'top': '-50px'};
    subs[2] = {'left': '0', 'top': '-70px'};
    subs[3] = {'left': '50px', 'top': '-50px'};
    subs[4] = {'left': '70px', 'top': '0px'};
    subs[5] = {'left': '50px', 'top': '50px'};
    subs[6] = {'left': '0px', 'top': '70px'};
    subs[7] = {'left': '-50px', 'top': '50px'};
    subs[8] = {'left': '-70px', 'top': '0px'};
    
    
    
    $(".middle .button > a").click(function(event){                            
                     
       var button = $(this).parent('.button');
       
       if($(button).find('.sub').length == 0) return true;
       
       if(!$(button).find('.sub li:first').is(':visible')){
           
           if(lock) return;
           
           $(".middle .button .sub li").hide().css({'top': '0px', 'left': '0px','opacity': '0'});
           
           var count = $(button).find('.sub li').length;
               count = count > 8 ? 8 : count; 
           
           var i = 1;
           lock = true;
           
           function next() {
                
                setTimeout(function() {
                    
                    if (i > count){ 
                        lock = false; 
                        return; 
                    }
                    if($.browser.version != '8.0')
                        $(button).find('.sub li:nth-child('+i+')').css('display','block').animate({left: subs[i]['left'], top: subs[i]['top'], opacity: 1},400);
                    else
                        $(button).find('.sub li:nth-child('+i+')').css({'display':'block','left':subs[i]['left'],'top': subs[i]['top'], 'opacity': 1});
                    
                    i++;
                    next();
                    
                }, 60);

            } 
            
            next();
            
       }else
           $(button).find('.sub li').hide().css({'top': '0px', 'left': '0px','opacity': '0'});
       
               
       return false;       
    });    
    
}

function loginBlock(block){
    
    $(".login:visible").animate({
        left: '30%',
        opacity: 0
    },'200','linear',function(){
        $(this).css('left','70%').css('display','none');
    });    
    $(block).css({opacity: 0, display: 'block',left: '70%'});
    fix_block_items_width('.input-prepend',['.add-on','button'],'input');
    $(block).find('.checker').show();
    $(block).animate({opacity: 1, left: '50%'},'200');
}

