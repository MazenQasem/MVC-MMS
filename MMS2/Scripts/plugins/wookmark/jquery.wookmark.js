
$.fn.wookmark = function(options) {
  
  if(!this.wookmarkOptions) {
    this.wookmarkOptions = $.extend( {
        container: $('body'),
        offset: 2,
        autoResize: false,
        itemWidth: $(this[0]).outerWidth(),
        resizeDelay: 50
      }, options);
  } else if(options) {
    this.wookmarkOptions = $.extend(this.wookmarkOptions, options);
  }
  
     if(!this.wookmarkColumns) {
    this.wookmarkColumns = null;
    this.wookmarkContainerWidth = null;
  }
  
     this.wookmarkLayout = function() {
         var columnWidth = this.wookmarkOptions.itemWidth + this.wookmarkOptions.offset;
    var containerWidth = this.wookmarkOptions.container.width();
    var columns = Math.floor((containerWidth+this.wookmarkOptions.offset)/columnWidth);
    var offset = Math.round((containerWidth - (columns*columnWidth-this.wookmarkOptions.offset))/2);
    
         var bottom = 0;
    if(this.wookmarkColumns != null && this.wookmarkColumns.length == columns) {
      bottom = this.wookmarkLayoutColumns(columnWidth, offset);
    } else {
      bottom = this.wookmarkLayoutFull(columnWidth, columns, offset);
    }
    
         this.wookmarkOptions.container.css('height', bottom+'px');
  };
  
  
  this.wookmarkLayoutFull = function(columnWidth, columns, offset) {
         var heights = [];
    while(heights.length < columns) {
      heights.push(0);
    }
    
         this.wookmarkColumns = [];
    while(this.wookmarkColumns.length < columns) {
      this.wookmarkColumns.push([]);
    }
    
         var item, top, left, i=0, k=0, length=this.length, shortest=null, shortestIndex=null, bottom = 0;
    for(; i<length; i++ ) {
      item = $(this[i]);
      
             shortest = null;
      shortestIndex = 0;
      for(k=0; k<columns; k++) {
        if(shortest == null || heights[k] < shortest) {
          shortest = heights[k];
          shortestIndex = k;
        }
      }
      
             item.css({
        position: 'absolute',
        top: shortest+'px',
        left: (shortestIndex*columnWidth + offset)+'px'
      });
      
             heights[shortestIndex] = shortest + item.outerHeight() + this.wookmarkOptions.offset;
      bottom = Math.max(bottom, heights[shortestIndex]);
      
      this.wookmarkColumns[shortestIndex].push(item);
    }
    
    return bottom;
  };
  
  
  this.wookmarkLayoutColumns = function(columnWidth, offset) {
    var heights = [];
    while(heights.length < this.wookmarkColumns.length) {
      heights.push(0);
    }
    
    var i=0, length = this.wookmarkColumns.length, column;
    var k=0, kLength, item;
    var bottom = 0;
    for(; i<length; i++) {
      column = this.wookmarkColumns[i];
      kLength = column.length;
      for(k=0; k<kLength; k++) {
        item = column[k];
        item.css({
          left: (i*columnWidth + offset)+'px',
          top: heights[i]+'px'
        });
        heights[i] += item.outerHeight() + this.wookmarkOptions.offset;
        
        bottom = Math.max(bottom, heights[i]);
      }
    }
    
    return bottom;
  };
  
     this.wookmarkResizeTimer = null;
  if(!this.wookmarkResizeMethod) {
    this.wookmarkResizeMethod = null;
  }
  if(this.wookmarkOptions.autoResize) {
         this.wookmarkOnResize = function(event) {
      if(this.wookmarkResizeTimer) {
        clearTimeout(this.wookmarkResizeTimer);
      }
      this.wookmarkResizeTimer = setTimeout($.proxy(this.wookmarkLayout, this), this.wookmarkOptions.resizeDelay)
    };
    
         if(!this.wookmarkResizeMethod) {
      this.wookmarkResizeMethod = $.proxy(this.wookmarkOnResize, this);
    }
    $(window).resize(this.wookmarkResizeMethod);
  };
  
  
  this.wookmarkClear = function() {
    if(this.wookmarkResizeTimer) {
      clearTimeout(this.wookmarkResizeTimer);
      this.wookmarkResizeTimer = null;
    }
    if(this.wookmarkResizeMethod) {
      $(window).unbind('resize', this.wookmarkResizeMethod);
    }
  };
  
     this.wookmarkLayout();
  
     this.show();
};
