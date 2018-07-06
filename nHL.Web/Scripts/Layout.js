$(document).ready(function (e) 
{
    //
    // Try to init the culture drop down list. 
    //
    try 
    {
        $("#_cultureDDL").msDropDown();
    }
    catch (e) 
    {
        alert(e.message);
    }
});
