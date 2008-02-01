// This script defines the client-side validator extension class @@CLIENTID@@_ClientSideEvaludator
// to validate an input within a specified range and is greater or less than another input.
//
// This class defines how the validation is carried and what to do afterwards.


// derive this class from BaseClientValidator
ClassHelper.extend(@@CLIENTID@@_ClientSideEvaluator, BaseClientValidator);

//constructor
function @@CLIENTID@@_ClientSideEvaluator()
{
    BaseClientValidator.call(this, 
            '@@INPUT_CLIENTID@@',
            '@@INPUT_NORMAL_BKCOLOR@@',
            '@@INPUT_INVALID_BKCOLOR@@',
            '@@INVALID_INPUT_INDICATOR_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_CLIENTID@@'),
            '@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@'),
            '@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@'),
            '@@IGNORE_EMPTY_VALUE@@'
    );
}


// override BaseClientValidator.OnEvaludate() 
// This function is called to evaluate the input
@@CLIENTID@@_ClientSideEvaluator.prototype.OnEvaluate = function()
{
    result = BaseClientValidator.prototype.OnEvaluate.call(this);
    
    if (result.OK==false)
    {
        return result;
    }
    
        
    compareCtrl = document.getElementById('@@COMPARE_INPUT_CLIENTID@@');
   
    result = new ValidationResult();
    
    if (this.input.value!=null && this.input.value!='' && !isNaN(this.input.value))
    {
        controlValue = parseInt(this.input.value);
            
        if (compareCtrl!=null && compareCtrl.value!='' &&  !isNaN(compareCtrl.value))
        {
            compareValue = parseInt(compareCtrl.value);
            result.OK = controlValue >= @@MIN_VALUE@@ && controlValue<= @@MAX_VALUE@@ && controlValue @@COMPARISON_OP@@ compareValue;
        }
        else
        {
            // "compare to" value is not entered... asumme this value is ok and wait until the other is entered.
            result.OK = true;
        }
    }   
    else
    {
          result.OK = false;  
    }
    
    if (result.OK == false)
    {
        if ('@@ERROR_MESSAGE@@' == null || '@@ERROR_MESSAGE@@'=='')
        {
            if ('@@COMPARISON_OP@@' == '>=')
                result.Message = '@@INPUT_NAME@@ must be between @@MIN_VALUE@@ and @@MAX_VALUE@@ and greater than @@COMPARE_TO_INPUT_NAME@@';
            else
                result.Message = '@@INPUT_NAME@@ must be between @@MIN_VALUE@@ and @@MAX_VALUE@@ and less than @@COMPARE_TO_INPUT_NAME@@';
        }
        else
            result.Message = '@@ERROR_MESSAGE@@';
        
    }
    
    
    return result;

};

//@@CLIENTID@@_ClientSideEvaluator.prototype.OnValidationPassed = function()
//{
//    //alert('Length validator: input is valid');
//    BaseClientValidator.prototype.OnValidationPassed.call(this);
//}

//@@CLIENTID@@_ClientSideEvaluator.prototype.OnValidationFailed = function(error)
//{
//    //alert('Length validator: input is valid : ' + error);
//    BaseClientValidator.prototype.OnValidationFailed.call(this, error);
//        
//}

//@@CLIENTID@@_ClientSideEvaluator.prototype.SetErrorMessage = function(result)
//{
//    BaseClientValidator.prototype.SetErrorMessage.call(this, result);
//    alert(result.Message);
//}
