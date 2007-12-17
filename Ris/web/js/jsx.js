// create a global $ function that acts as an alias for document.getElementById
var $ = function(id) { return document.getElementById(id); }

var membersOf = function(obj) { var a = []; for(var k in obj) a.add(k); return a; }


/*
    Augments the javascript Array prototype with a number of convenience and functional-style methods, and some events.
    The following methods are added:
        add
        each
        map
        reduce
        select
        removeAt
        remove
        isArray
        
    The following events are added:
        itemAdded (note: this event is only fired when the "add" method is called - assignment by [] does not invoke this event)
        itemRemoved
*/

// isArray always returns true - provides an easy way to test if an unknown object is an instance of an array or not
if(!Array.prototype.isArray)
{
    Array.prototype.isArray = true;
}

// adds an item to the end of the array
if(!Array.prototype.add){
	Array.prototype.add = function(obj)
	{
		var i = this.length;
		this[i] = obj;
		if(this.itemAdded)
			this.itemAdded(this, {item: obj, index: i});
	};
}

// iterates over the array, passing each element to the supplied function
if(!Array.prototype.each)
{
    Array.prototype.each = function(func)
    {
        for(var i = 0; i < this.length; i++)
            func(this[i]);
    }
}

// maps this array onto a new array, using the the supplied mapping function
if(!Array.prototype.map)
{
    Array.prototype.map = function(func)
    {
        var result = [];
        for(var i = 0; i < this.length; i++)
            result[i] = func(this[i]);
        return result;
    }
}

// reduces this array to a scalar value by calling the specified function for each item in the array,
// an taking the return value of the function as the next value of the "accumlator"
//      initial - the initial value of the accumlator
//      func - a function of the form func(accumlator, element), that returns a new value for the accumlator
// e.g. if x is an array of ints, then sum(x) = x.reduce(0, function(sum, y) { return sum + y; });
if(!Array.prototype.reduce)
{
    Array.prototype.reduce = function(initial, func)
    {
        var memo = initial;
        for(var i = 0; i < this.length; i++)
            memo = func(memo, this[i]);
        return memo;
    }
}

// returns a new array containing only those elements of this array that satisfy the specified predicate function
if(!Array.prototype.select)
{
    Array.prototype.select = function(func)
    {
        var result = [];
        for(var i = 0; i < this.length; i++)
            if(func(this[i]))
                result.push(this[i]);
        return result;
    }
}

// returns the first element of this array that satisfies the specified predicate function, or null
if(!Array.prototype.find)
{
    Array.prototype.find = function(func)
    {
        for(var i = 0; i < this.length; i++)
            if(func(this[i]))
                return this[i];
        return null;
    }
}

// returns the index of the specified object, or -1 if not found
if(!Array.prototype.indexOf)
{
	Array.prototype.indexOf = function(obj)
	{
        for(var i = 0; i < this.length; i++)
            if(this[i] == obj)
                return i;
        return -1;
	};
}


// removes the item at the specified index
if(!Array.prototype.removeAt)
{
	Array.prototype.removeAt = function(i)
	{
		var obj = this.splice(i, 1);
		if(this.itemRemoved)
			this.itemRemoved(this, {item: obj, index: i});
		return obj;
	};
}

// removes the specified item from the array, or does nothing if the item is not contained in this array
if(!Array.prototype.remove)
{
	Array.prototype.remove = function(obj)
	{
		var i = this.indexOf(obj);
		return (i > -1) ? this.removeAt(i) : null;
	};
}

if(!String.prototype.escapeHTML)
{
    // from Prototype.js library (www.prototypejs.org)
    String.prototype.escapeHTML = function()
    {
        var div = document.createElement('div');
        var text = document.createTextNode(this);
        div.appendChild(text);
        return div.innerHTML;
    }
    
    // from Prototype.js library (www.prototypejs.org)
    String.prototype.unescapeHTML = function()
    {
        var div = document.createElement('div');
        div.innerHTML = this.stripTags();
        return div.childNodes[0].nodeValue;
    }
}

// utility to combine a list of strings with separator
if (!String.combine)
{
	String.combine = function(values, separator)
	{
		separator = separator ? (separator + "").escapeHTML() : "";

		if (values == null || values.length == 0)
			return "";
			
		return values.reduce("", 
			function(memo, item) 
			{
				return memo.length == 0 ? item : memo + separator + item;
			});
	};
}

// add some decent date serialization support
if(!Date.prototype.toISOString)
{
    Date.prototype.toISOString = function () {

        function f(n) {
            // Format integers to have at least two digits.
            return n < 10 ? '0' + n : n;
        }

        return this.getFullYear() + '-' +
                f(this.getMonth() + 1) + '-' +
                f(this.getDate()) + 'T' +
                f(this.getHours()) + ':' +
                f(this.getMinutes()) + ':' +
                f(this.getSeconds());
    }
    
    Date.parseISOString = function(isoDateString)
    {
        var y = isoDateString.substring(0, 4);
        var m = isoDateString.substring(5, 7) - 1;
        var d = isoDateString.substring(8, 10);
        var h = isoDateString.substring(11, 13);
        var n = isoDateString.substring(14, 16);
        var s = isoDateString.substring(17, 19);
        
        var date = new Date();
        date.setFullYear(y);
        date.setMonth(m);
        date.setDate(d);
        date.setHours(h, n, s);
        return date;
   };
}
    
if (!Date.prototype.addYears)
{
	Date.prototype.addYears = function(offset)
	{
		var newDate = new Date(this);
		newDate.setYear(newDate.getYear() + offset);	
		return newDate;
	};

	Date.prototype.addMonths = function(offset)
	{
		var newDate = new Date(this);
		newDate.setMonth(newDate.getMonth() + offset);	
		return newDate;
	};

	Date.prototype.addDays = function(offset)
	{
		var newDate = new Date(this);
		newDate.setDate(newDate.getDate() + offset);	
		return newDate;
	};

	Date.prototype.addHours = function(offset)
	{
		var newDate = new Date(this);
		newDate.setHours(newDate.getHours() + offset);	
		return newDate;
	};

	Date.prototype.addMinutes = function(offset)
	{
		var newDate = new Date(this);
		newDate.setMinutes(newDate.getMinutes() + offset);	
		return newDate;
	};

	Date.prototype.addSeconds = function(offset)
	{
		var newDate = new Date(this);
		newDate.setSeconds(newDate.getSeconds() + offset);	
		return newDate;
	};

	Date.prototype.addMilliseconds = function(offset)
	{
		var newDate = new Date(this);
		newDate.setMilliseconds(newDate.getMilliseconds() + offset);	
		return newDate;
	};
}

if (!Date.today)
{
	Date.today = function()
	{
		var today = new Date();
		today.setHours(0);
		today.setMinutes(0);
		today.setSeconds(0);
		today.setMilliseconds(0);
		return today;
	};
}

if (!Date.compare)
{
	// compare two date, a null date object is infinite into the future
	Date.compare = function(date1, date2)
	{
		if (date1 == null && date2 == null)
			return 0;
		else if (date1 == null)
			return 1;
		else if (date2 == null)
			return -1;
		else
			return Date.parse(date1) - Date.parse(date2);
	}

	// Compare whether date1 is more recent than date2.
	// Any time after the beginning of today is considered more recent than the past.
	// Time of null is considered infinite into the future.
	Date.compareMoreRecent = function(date1, date2)
	{
		if (date1 == date2)  // also take care of both equal null
			return 0;

		var today = Date.today();
		var dateOneMoreRecent = false;
		
		if (date1 == null)
		{
			if (Date.compare(date2, today) < 0)
				dateOneMoreRecent = true;  // time1 in the future, time2 in the past
		}
		else if (date2 == null)
		{
			if (Date.compare(date1, today) < 0)
				dateOneMoreRecent = false;  // time1 in the past, time2 in the future
		}
		else // both are not null
		{
			var timeOneSpan = Date.parse(date1) - Date.parse(today);
			var timeTwoSpan = Date.parse(date2) - Date.parse(today);

			if (timeOneSpan > 0 && timeTwoSpan > 0)
			{
				// Both in the future
				dateOneMoreRecent = timeOneSpan < timeTwoSpan;
			}
			else if (timeOneSpan < 0 && timeTwoSpan < 0)
			{
				// Both in the past
				dateOneMoreRecent = timeOneSpan > timeTwoSpan;
			}
			else
			{
				dateOneMoreRecent = timeOneSpan > timeTwoSpan;
			}
		}

		return dateOneMoreRecent ? -1 : 1;
	}
}
