﻿

    
(function($) {

         
  $.cleditor = {

         defaultOptions: {
      width:        500,        height:       250,        controls:                          "bold italic underline strikethrough subscript superscript | font size " +
                    "style | color highlight removeformat | bullets numbering | outdent " +
                    "indent | alignleft center alignright justify | undo redo | " +
                    "rule image link unlink | cut copy paste pastetext | print source",
      colors:                            "FFF FCC FC9 FF9 FFC 9F9 9FF CFF CCF FCF " +
                    "CCC F66 F96 FF6 FF3 6F9 3FF 6FF 99F F9F " +
                    "BBB F00 F90 FC6 FF0 3F3 6CC 3CF 66C C6C " +
                    "999 C00 F60 FC3 FC0 3C0 0CC 36F 63F C3C " +
                    "666 900 C60 C93 990 090 399 33F 60C 939 " +
                    "333 600 930 963 660 060 366 009 339 636 " +
                    "000 300 630 633 330 030 033 006 309 303",    
      fonts:                             "Arial,Arial Black,Comic Sans MS,Courier New,Narrow,Garamond," +
                    "Georgia,Impact,Sans Serif,Serif,Tahoma,Trebuchet MS,Verdana",
      sizes:                             "1,2,3,4,5,6,7",
      styles:                            [["Paragraph", "<p>"], ["Header 1", "<h1>"], ["Header 2", "<h2>"],
                    ["Header 3", "<h3>"],  ["Header 4","<h4>"],  ["Header 5","<h5>"],
                    ["Header 6","<h6>"]],
      useCSS:       false,        docType:                           '<!DOCTYPE HTML PUBLIC "-       docCSSFile:                        "", 
      bodyStyle:                         "margin:10px; font:10pt Arial,Verdana; cursor:text"
    },

                        buttons: {
             init:
      "bold,,|" +
      "italic,,|" +
      "underline,,|" +
      "strikethrough,,|" +
      "subscript,,|" +
      "superscript,,|" +
      "font,,fontname,|" +
      "size,Font Size,fontsize,|" +
      "style,,formatblock,|" +
      "color,Font Color,forecolor,|" +
      "highlight,Text Highlight Color,hilitecolor,color|" +
      "removeformat,Remove Formatting,|" +
      "bullets,,insertunorderedlist|" +
      "numbering,,insertorderedlist|" +
      "outdent,,|" +
      "indent,,|" +
      "alignleft,Align Text Left,justifyleft|" +
      "center,,justifycenter|" +
      "alignright,Align Text Right,justifyright|" +
      "justify,,justifyfull|" +
      "undo,,|" +
      "redo,,|" +
      "rule,Insert Horizontal Rule,inserthorizontalrule|" +
      "image,Insert Image,insertimage,url|" +
      "link,Insert Hyperlink,createlink,url|" +
      "unlink,Remove Hyperlink,|" +
      "cut,,|" +
      "copy,,|" +
      "paste,,|" +
      "pastetext,Paste as Text,inserthtml,|" +
      "print,,|" +
      "source,Show Source"
    },

         imagesPath: function() { return imagesPath(); }

  };

     $.fn.cleditor = function(options) {

         var $result = $([]);

         this.each(function(idx, elem) {
      if (elem.tagName == "TEXTAREA") {
        var data = $.data(elem, CLEDITOR);
        if (!data) data = new cleditor(elem, options);
        $result = $result.add(data);
      }
    });

         return $result;

  };
    
         
  var

     BACKGROUND_COLOR = "backgroundColor",
  BUTTON           = "button",
  BUTTON_NAME      = "buttonName",
  CHANGE           = "change",
  CLEDITOR         = "cleditor",
  CLICK            = "click",
  DISABLED         = "disabled",
  DIV_TAG          = "<div>",
  TRANSPARENT      = "transparent",
  UNSELECTABLE     = "unselectable",

     MAIN_CLASS       = "cleditorMain",       TOOLBAR_CLASS    = "cleditorToolbar",    GROUP_CLASS      = "cleditorGroup",      BUTTON_CLASS     = "cleditorButton",     DISABLED_CLASS   = "cleditorDisabled",   DIVIDER_CLASS    = "cleditorDivider",    POPUP_CLASS      = "cleditorPopup",      LIST_CLASS       = "cleditorList",       COLOR_CLASS      = "cleditorColor",      PROMPT_CLASS     = "cleditorPrompt",     MSG_CLASS        = "cleditorMsg",      
     ie = $.browser.msie,
  ie6 = /msie\s6/i.test(navigator.userAgent),

     iOS = /iphone|ipad|ipod/i.test(navigator.userAgent),

     popups = {},

     documentClickAssigned,

     buttons = $.cleditor.buttons;

         
           $.each(buttons.init.split("|"), function(idx, button) {
    var items = button.split(","), name = items[0];
    buttons[name] = {
      stripIndex: idx,
      name: name,
      title: items[1] === "" ? name.charAt(0).toUpperCase() + name.substr(1) : items[1],
      command: items[2] === "" ? name : items[2],
      popupName: items[3] === "" ? name : items[3]
    };
  });
  delete buttons.init;

         
     cleditor = function(area, options) {

    var editor = this;

         editor.options = options = $.extend({}, $.cleditor.defaultOptions, options);

         var $area = editor.$area = $(area)
      .hide()
      .data(CLEDITOR, editor)
      .blur(function() {
                 updateFrame(editor, true);
      });

         var $main = editor.$main = $(DIV_TAG)
      .addClass(MAIN_CLASS)
      .width(options.width)
      .height(options.height);

         var $toolbar = editor.$toolbar = $(DIV_TAG)
      .addClass(TOOLBAR_CLASS)
      .appendTo($main);

         var $group = $(DIV_TAG)
      .addClass(GROUP_CLASS)
      .appendTo($toolbar);
    
         $.each(options.controls.split(" "), function(idx, buttonName) {
      if (buttonName === "") return true;

             if (buttonName == "|") {

                 var $div = $(DIV_TAG)
          .addClass(DIVIDER_CLASS)
          .appendTo($group);

                 $group = $(DIV_TAG)
          .addClass(GROUP_CLASS)
          .appendTo($toolbar);

      }

             else {
        
                 var button = buttons[buttonName];

                 var $buttonDiv = $(DIV_TAG)
          .data(BUTTON_NAME, button.name)
          .addClass(BUTTON_CLASS)
          .attr("title", button.title)
          .bind(CLICK, $.proxy(buttonClick, editor))
          .appendTo($group)
          .hover(hoverEnter, hoverLeave);

                 var map = {};
        if (button.css) map = button.css;
        else if (button.image) map.backgroundImage = imageUrl(button.image);
        if (button.stripIndex) map.backgroundPosition = button.stripIndex * -24;
        $buttonDiv.css(map);

                 if (ie)
          $buttonDiv.attr(UNSELECTABLE, "on");

                 if (button.popupName)
          createPopup(button.popupName, options, button.popupClass,
            button.popupContent, button.popupHover);
        
      }

    });

         $main.insertBefore($area)
      .append($area);

         if (!documentClickAssigned) {
      $(document).click(function(e) {
                 var $target = $(e.target);
        if (!$target.add($target.parents()).is("." + PROMPT_CLASS))
          hidePopups();
      });
      documentClickAssigned = true;
    }

         if (/auto|%/.test("" + options.width + options.height))
      $(window).resize(function() {refresh(editor);});

         refresh(editor);

  };

         
  var fn = cleditor.prototype,

           methods = [
    ["clear", clear],
    ["disable", disable],
    ["execCommand", execCommand],
    ["focus", focus],
    ["hidePopups", hidePopups],
    ["sourceMode", sourceMode, true],
    ["refresh", refresh],
    ["select", select],
    ["selectedHTML", selectedHTML, true],
    ["selectedText", selectedText, true],
    ["showMessage", showMessage],
    ["updateFrame", updateFrame],
    ["updateTextArea", updateTextArea]
  ];

  $.each(methods, function(idx, method) {
    fn[method[0]] = function() {
      var editor = this, args = [editor];
             for(var x = 0; x < arguments.length; x++) {args.push(arguments[x]);}
      var result = method[1].apply(editor, args);
      if (method[2]) return result;
      return editor;
    };
  });

     fn.change = function(handler) {
    var $this = $(this);
    return handler ? $this.bind(CHANGE, handler) : $this.trigger(CHANGE);
  };

         
     function buttonClick(e) {

    var editor = this,
        buttonDiv = e.target,
        buttonName = $.data(buttonDiv, BUTTON_NAME),
        button = buttons[buttonName],
        popupName = button.popupName,
        popup = popups[popupName];

         if (editor.disabled || $(buttonDiv).attr(DISABLED) == DISABLED)
      return;

         var data = {
      editor: editor,
      button: buttonDiv,
      buttonName: buttonName,
      popup: popup,
      popupName: popupName,
      command: button.command,
      useCSS: editor.options.useCSS
    };

    if (button.buttonClick && button.buttonClick(e, data) === false)
      return false;

         if (buttonName == "source") {

             if (sourceMode(editor)) {
        delete editor.range;
        editor.$area.hide();
        editor.$frame.show();
        buttonDiv.title = button.title;
      }

             else {
        editor.$frame.hide();
        editor.$area.show();
        buttonDiv.title = "Show Rich Text";
      }

                    setTimeout(function() {refreshButtons(editor);}, 100);

    }

         else if (!sourceMode(editor)) {

             if (popupName) {
        var $popup = $(popup);

                 if (popupName == "url") {

                     if (buttonName == "link" && selectedText(editor) === "") {
            showMessage(editor, "A selection is required when inserting a link.", buttonDiv);
            return false;
          }

                     $popup.children(":button")
            .unbind(CLICK)
            .bind(CLICK, function() {

                             var $text = $popup.find(":text"),
                url = $.trim($text.val());
              if (url !== "")
                execCommand(editor, data.command, url, null, data.button);

                             $text.val("http:               hidePopups();
              focus(editor);

            });

        }

                 else if (popupName == "pastetext") {

                     $popup.children(":button")
            .unbind(CLICK)
            .bind(CLICK, function() {

                             var $textarea = $popup.find("textarea"),
                text = $textarea.val().replace(/\n/g, "<br />");
              if (text !== "")
                execCommand(editor, data.command, text, null, data.button);

                             $textarea.val("");
              hidePopups();
              focus(editor);

            });

        }

                 if (buttonDiv !== $.data(popup, BUTTON)) {
          showPopup(editor, popup, buttonDiv);
          return false;          }

                 return;

      }

             else if (buttonName == "print")
        editor.$frame[0].contentWindow.print();

             else if (!execCommand(editor, data.command, data.value, data.useCSS, buttonDiv))
        return false;

    }

         focus(editor);

  }

     function hoverEnter(e) {
    var $div = $(e.target).closest("div");
    $div.css(BACKGROUND_COLOR, $div.data(BUTTON_NAME) ? "#FFF" : "#FFC");
  }

     function hoverLeave(e) {
    $(e.target).closest("div").css(BACKGROUND_COLOR, "transparent");
  }

     function popupClick(e) {

    var editor = this,
        popup = e.data.popup,
        target = e.target;

         if (popup === popups.msg || $(popup).hasClass(PROMPT_CLASS))
      return;

         var buttonDiv = $.data(popup, BUTTON),
        buttonName = $.data(buttonDiv, BUTTON_NAME),
        button = buttons[buttonName],
        command = button.command,
        value,
        useCSS = editor.options.useCSS;

         if (buttonName == "font")
             value = target.style.fontFamily.replace(/"/g, "");
    else if (buttonName == "size") {
      if (target.tagName == "DIV")
        target = target.children[0];
      value = target.innerHTML;
    }
    else if (buttonName == "style")
      value = "<" + target.tagName + ">";
    else if (buttonName == "color")
      value = hex(target.style.backgroundColor);
    else if (buttonName == "highlight") {
      value = hex(target.style.backgroundColor);
      if (ie) command = 'backcolor';
      else useCSS = true;
    }

         var data = {
      editor: editor,
      button: buttonDiv,
      buttonName: buttonName,
      popup: popup,
      popupName: button.popupName,
      command: command,
      value: value,
      useCSS: useCSS
    };

    if (button.popupClick && button.popupClick(e, data) === false)
      return;

         if (data.command && !execCommand(editor, data.command, data.value, data.useCSS, buttonDiv))
      return false;

         hidePopups();
    focus(editor);

  }

         
     function checksum(text)
  {
    var a = 1, b = 0;
    for (var index = 0; index < text.length; ++index) {
      a = (a + text.charCodeAt(index)) % 65521;
      b = (b + a) % 65521;
    }
    return (b << 16) | a;
  }

     function clear(editor) {
    editor.$area.val("");
    updateFrame(editor);
  }

     function createPopup(popupName, options, popupTypeClass, popupContent, popupHover) {

         if (popups[popupName])
      return popups[popupName];

         var $popup = $(DIV_TAG)
      .hide()
      .addClass(POPUP_CLASS)
      .appendTo("body");

     
         if (popupContent)
      $popup.html(popupContent);

         else if (popupName == "color") {
      var colors = options.colors.split(" ");
      if (colors.length < 10)
        $popup.width("auto");
      $.each(colors, function(idx, color) {
        $(DIV_TAG).appendTo($popup)
          .css(BACKGROUND_COLOR, "#" + color);
      });
      popupTypeClass = COLOR_CLASS;
    }

         else if (popupName == "font")
      $.each(options.fonts.split(","), function(idx, font) {
        $(DIV_TAG).appendTo($popup)
          .css("fontFamily", font)
          .html(font);
      });

         else if (popupName == "size")
      $.each(options.sizes.split(","), function(idx, size) {
        $(DIV_TAG).appendTo($popup)
          .html("<font size=" + size + ">" + size + "</font>");
      });

         else if (popupName == "style")
      $.each(options.styles, function(idx, style) {
        $(DIV_TAG).appendTo($popup)
          .html(style[1] + style[0] + style[1].replace("<", "</"));
      });

         else if (popupName == "url") {
      $popup.html('Enter URL:<br><input type=text value="http:       popupTypeClass = PROMPT_CLASS;
    }

         else if (popupName == "pastetext") {
      $popup.html('Paste your content here and click submit.<br /><textarea cols=40 rows=3></textarea><br /><input type=button value=Submit>');
      popupTypeClass = PROMPT_CLASS;
    }

         if (!popupTypeClass && !popupContent)
      popupTypeClass = LIST_CLASS;
    $popup.addClass(popupTypeClass);

         if (ie) {
      $popup.attr(UNSELECTABLE, "on")
        .find("div,font,p,h1,h2,h3,h4,h5,h6")
        .attr(UNSELECTABLE, "on");
    }

         if ($popup.hasClass(LIST_CLASS) || popupHover === true)
      $popup.children().hover(hoverEnter, hoverLeave);

         popups[popupName] = $popup[0];
    return $popup[0];

  }

     function disable(editor, disabled) {

         if (disabled) {
      editor.$area.attr(DISABLED, DISABLED);
      editor.disabled = true;
    }
    else {
      editor.$area.removeAttr(DISABLED);
      delete editor.disabled;
    }

                   try {
      if (ie) editor.doc.body.contentEditable = !disabled;
      else editor.doc.designMode = !disabled ? "on" : "off";
    }
              catch (err) {}

         refreshButtons(editor);

  }

     function execCommand(editor, command, value, useCSS, button) {

         restoreRange(editor);

         if (!ie) {
      if (useCSS === undefined || useCSS === null)
        useCSS = editor.options.useCSS;
      editor.doc.execCommand("styleWithCSS", 0, useCSS.toString());
    }

         var success = true, description;
    if (ie && command.toLowerCase() == "inserthtml")
      getRange(editor).pasteHTML(value);
    else {
      try { success = editor.doc.execCommand(command, 0, value || null); }
      catch (err) { description = err.description; success = false; }
      if (!success) {
        if ("cutcopypaste".indexOf(command) > -1)
          showMessage(editor, "For security reasons, your browser does not support the " +
            command + " command. Try using the keyboard shortcut or context menu instead.",
            button);
        else
          showMessage(editor,
            (description ? description : "Error executing the " + command + " command."),
            button);
      }
    }

         refreshButtons(editor);
    return success;

  }

     function focus(editor) {
    setTimeout(function() {
      if (sourceMode(editor)) editor.$area.focus();
      else editor.$frame[0].contentWindow.focus();
      refreshButtons(editor);
    }, 0);
  }

     function getRange(editor) {
    if (ie) return getSelection(editor).createRange();
    return getSelection(editor).getRangeAt(0);
  }

     function getSelection(editor) {
    if (ie) return editor.doc.selection;
    return editor.$frame[0].contentWindow.getSelection();
  }

              function hex(s) {
    var m = /rgba?\((\d+), (\d+), (\d+)/.exec(s),
      c = s.split("");
    if (m) {
      s = ( m[1] << 16 | m[2] << 8 | m[3] ).toString(16);
      while (s.length < 6)
        s = "0" + s;
    }
    return "#" + (s.length == 6 ? s : c[1] + c[1] + c[2] + c[2] + c[3] + c[3]);
  }

     function hidePopups() {
    $.each(popups, function(idx, popup) {
      $(popup)
        .hide()
        .unbind(CLICK)
        .removeData(BUTTON);
    });
  }

     function imagesPath() {
    var cssFile = "jquery.cleditor.css",
        href = $("link[href$='" + cssFile +"']").attr("href");
    return href.substr(0, href.length - cssFile.length) + "images/";
  }

     function imageUrl(filename) {
    return "url(" + imagesPath() + filename + ")";
  }

     function refresh(editor) {

    var $main = editor.$main,
      options = editor.options;

         if (editor.$frame) 
      editor.$frame.remove();

         var $frame = editor.$frame = $('<iframe frameborder="0" src="javascript:true;">')
      .hide()
      .appendTo($main);

         var contentWindow = $frame[0].contentWindow,
      doc = editor.doc = contentWindow.document,
      $doc = $(doc);

    doc.open();
    doc.write(
      options.docType +
      '<html>' +
      ((options.docCSSFile === '') ? '' : '<head><link rel="stylesheet" type="text/css" href="' + options.docCSSFile + '" /></head>') +
      '<body style="' + options.bodyStyle + '"></body></html>'
    );
    doc.close();

              if (ie)
      $doc.click(function() {focus(editor);});

         updateFrame(editor);

         if (ie) {

                           $doc.bind("beforedeactivate beforeactivate selectionchange keypress", function(e) {
        
                 if (e.type == "beforedeactivate")
          editor.inactive = true;
        
                 else if (e.type == "beforeactivate") {
          if (!editor.inactive && editor.range && editor.range.length > 1)
            editor.range.shift();
          delete editor.inactive;
        }

                 else if (!editor.inactive) {
          if (!editor.range) 
            editor.range = [];
          editor.range.unshift(getRange(editor));

                     while (editor.range.length > 2)
            editor.range.pop();
        }

      });

             $frame.focus(function() {
        restoreRange(editor);
      });

    }

         ($.browser.mozilla ? $doc : $(contentWindow)).blur(function() {
      updateTextArea(editor, true);
    });

         $doc.click(hidePopups)
      .bind("keyup mouseup", function() {
        refreshButtons(editor);
      });

              if (iOS) editor.$area.show();
    else $frame.show();

         $(function() {

      var $toolbar = editor.$toolbar,
          $group = $toolbar.children("div:last"),
          wid = $main.width();

             var hgt = $group.offset().top + $group.outerHeight() - $toolbar.offset().top + 1;
      $toolbar.height(hgt);

             hgt = (/%/.test("" + options.height) ? $main.height() : parseInt(options.height)) - hgt;
      $frame.width(wid).height(hgt);

                    editor.$area.width(wid).height(ie6 ? hgt - 2 : hgt);

             disable(editor, editor.disabled);

             refreshButtons(editor);

    });

  }

     function refreshButtons(editor) {

         if (!iOS && $.browser.webkit && !editor.focused) {
      editor.$frame[0].contentWindow.focus();
      window.focus();
      editor.focused = true;
    }

         var queryObj = editor.doc;
    if (ie) queryObj = getRange(editor);

         var inSourceMode = sourceMode(editor);
    $.each(editor.$toolbar.find("." + BUTTON_CLASS), function(idx, elem) {

      var $elem = $(elem),
        button = $.cleditor.buttons[$.data(elem, BUTTON_NAME)],
        command = button.command,
        enabled = true;

             if (editor.disabled)
        enabled = false;
      else if (button.getEnabled) {
        var data = {
          editor: editor,
          button: elem,
          buttonName: button.name,
          popup: popups[button.popupName],
          popupName: button.popupName,
          command: button.command,
          useCSS: editor.options.useCSS
        };
        enabled = button.getEnabled(data);
        if (enabled === undefined)
          enabled = true;
      }
      else if (((inSourceMode || iOS) && button.name != "source") ||
      (ie && (command == "undo" || command == "redo")))
        enabled = false;
      else if (command && command != "print") {
        if (ie && command == "hilitecolor")
          command = "backcolor";
                 if (!ie || command != "inserthtml") {
          try {enabled = queryObj.queryCommandEnabled(command);}
          catch (err) {enabled = false;}
        }
      }

             if (enabled) {
        $elem.removeClass(DISABLED_CLASS);
        $elem.removeAttr(DISABLED);
      }
      else {
        $elem.addClass(DISABLED_CLASS);
        $elem.attr(DISABLED, DISABLED);
      }

    });
  }

     function restoreRange(editor) {
    if (ie && editor.range)
      editor.range[0].select();
  }

     function select(editor) {
    setTimeout(function() {
      if (sourceMode(editor)) editor.$area.select();
      else execCommand(editor, "selectall");
    }, 0);
  }

     function selectedHTML(editor) {
    restoreRange(editor);
    var range = getRange(editor);
    if (ie)
      return range.htmlText;
    var layer = $("<layer>")[0];
    layer.appendChild(range.cloneContents());
    var html = layer.innerHTML;
    layer = null;
    return html;
  }

     function selectedText(editor) {
    restoreRange(editor);
    if (ie) return getRange(editor).text;
    return getSelection(editor).toString();
  }

     function showMessage(editor, message, button) {
    var popup = createPopup("msg", editor.options, MSG_CLASS);
    popup.innerHTML = message;
    showPopup(editor, popup, button);
  }

     function showPopup(editor, popup, button) {

    var offset, left, top, $popup = $(popup);

         if (button) {
      var $button = $(button);
      offset = $button.offset();
      left = --offset.left;
      top = offset.top + $button.height();
    }
    else {
      var $toolbar = editor.$toolbar;
      offset = $toolbar.offset();
      left = Math.floor(($toolbar.width() - $popup.width()) / 2) + offset.left;
      top = offset.top + $toolbar.height() - 2;
    }

         hidePopups();
    $popup.css({left: left, top: top})
      .show();

         if (button) {
      $.data(popup, BUTTON, button);
      $popup.bind(CLICK, {popup: popup}, $.proxy(popupClick, editor));
    }

         setTimeout(function() {
      $popup.find(":text,textarea").eq(0).focus().select();
    }, 100);

  }

     function sourceMode(editor) {
    return editor.$area.is(":visible");
  }

     function updateFrame(editor, checkForChange) {
    
    var code = editor.$area.val(),
      options = editor.options,
      updateFrameCallback = options.updateFrame,
      $body = $(editor.doc.body);

              if (updateFrameCallback) {
      var sum = checksum(code);
      if (checkForChange && editor.areaChecksum == sum)
        return;
      editor.areaChecksum = sum;
    }

         var html = updateFrameCallback ? updateFrameCallback(code) : code;

         html = html.replace(/<(?=\/?script)/ig, "&lt;");

         if (options.updateTextArea)
      editor.frameChecksum = checksum(html);

         if (html != $body.html()) {
      $body.html(html);
      $(editor).triggerHandler(CHANGE);
    }

  }

     function updateTextArea(editor, checkForChange) {

    var html = $(editor.doc.body).html(),
      options = editor.options,
      updateTextAreaCallback = options.updateTextArea,
      $area = editor.$area;

              if (updateTextAreaCallback) {
      var sum = checksum(html);
      if (checkForChange && editor.frameChecksum == sum)
        return;
      editor.frameChecksum = sum;
    }

         var code = updateTextAreaCallback ? updateTextAreaCallback(html) : html;

         if (options.updateFrame)
      editor.areaChecksum = checksum(code);

         if (code != $area.val()) {
      $area.val(code);
      $(editor).triggerHandler(CHANGE);
    }

  }

})(jQuery);
