var currentTab = 0; // Current tab is set to be the first tab (0)
var emailResult = false;
var creditCardResult = false;
var carResult = {};
showTab(currentTab); // Display the current tab

$("#loader").bind("ajaxStart", function () {
    $(this).show();
}).bind("ajaxStop", function () {
    $(this).hide();
});

function showTab(n) {
    // This function will display the specified tab of the form ...
    var x = document.getElementsByClassName("tab");
    x[n].style.display = "block";
    // ... and fix the Previous/Next buttons:
    if (n == 0) {
        document.getElementById("prevBtn").style.display = "none";
    } else {
        document.getElementById("prevBtn").style.display = "inline";
    }
    if (n == (x.length - 1)) {
        document.getElementById("nextBtn").innerHTML = "Submit";
    } else {
        document.getElementById("nextBtn").innerHTML = "Next";
    }
    // ... and run a function that displays the correct step indicator:
    fixStepIndicator(n)
}

function nextPrev(n) {
    // This function will figure out which tab to display
    var x = document.getElementsByClassName("tab");
    // Exit the function if any field in the current tab is invalid:
    if (n == 1 && !validateForm()) return false;
    // Hide the current tab:
    x[currentTab].style.display = "none";
    // Increase or decrease the current tab by 1:
    currentTab = currentTab + n;


    // if you have reached the end of the form... :
    if (currentTab >= x.length) {
        //...the form gets submitted:
        document.getElementById("regForm").style.display = "none";
        $('#finishingBox').show();
        document.getElementById("regForm").submit();
        
        return false;
    }
    // Otherwise, display the correct tab:
    showTab(currentTab);
}




function validateEmail(mail) {
    if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,6})+$/.test(mail)) {
        return (true)
    }
    return (false)
}

function CheckPassword(inputtxt) {
    if (/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9])(?!.*\s).{8,60}$/.test(inputtxt)) {
        return (true)
    }
    return (false)
}

function getResult(input) {
    return input;
}

async function doCheckEmail(email) {
    const result = await $.ajax({
        url: "?handler=CheckEmailExists",
        data: { email: email },
        type: "GET"
    });
    return result;
}

function checkEmailExists(email) {   
        $.ajax({
            url: "?handler=CheckEmailExists",
            data: { email: email },
            dataType: 'JSON',
            type: "GET",
            async: false,
            cache: true,
            beforeSend: function () {
                console.log('starting checking email...');
            },
            success: function (data) {
                console.log('email data: ' + data);
                emailResult = data;
            },
            error: function () {
                console.log("Error");
            },
            complete: function () {
                console.log('finished checking email...');
            },
        });    
    $('#checkingEmailText').fadeOut(500);
    return emailResult;
}

function checkCreditCard(cardNumber, expiry, cvc) {    
    console.log('card number: ' + cardNumber + ' Expiry: ' + expiry + ' cvc: ' + cvc);
    $.ajax({
        url: "?handler=CheckCreditCard",
        type: "GET",
        async: false,
        data: { cardNumber: cardNumber, expiry: expiry, cvc: cvc },
        dataType: 'json',
        beforeSend: function () {

        },
        success: function (data) {
            creditCardResult = data;            
        },
        error: function () {
            console.log("Error");
        },
        coplete: function () {
            creditCardResult = data;
        }
    });
    return creditCardResult;
}

function checkCar(registrationPlate) {
    var settings = {
        "url": "https://api.checkcardetails.co.uk/vehicledata/vehicleregistration?apikey=dfe93ef0930ff5b2f9aab427f224036f&vrm=" + registrationPlate,
        "method": "GET",
        "timeout": 0,
    };   


    return true;
}




function validateForm() {
    // This function deals with validation of the form fields
    var x, y, i, valid = true;
    x = document.getElementsByClassName("tab");
    y = x[currentTab].getElementsByTagName("input");
    // A loop that checks every input field in the current tab: 
    for (i = 0; i < y.length; i++) {
        if (currentTab == 0) {
            if (y[0].value != "") {
                if (!validateEmail(y[0].value)) {

                    y[i].className += " invalid";
                    document.getElementById('emailError').innerHTML = "<i class=\"fa fa-times-circle\"></i> E-Mail is not in correct format";
                    valid = false;
                }
                else {
                    if (checkEmailExists(y[0].value)) {
                        y[i].className = 'invalidEmail';
                        document.getElementById('emailError').innerHTML = "<i class=\"fa fa-times-circle\"></i> E-Mail already taken... Are you trying to <a href=\"/Identity/Account/Login\" style=\"text-decoration: none\" ><span class=\"text-warning\"> log in? </span> </a>";
                        valid = false;
                    }
                    else {
                        document.getElementById('emailError').innerHTML = "";
                        y[i].className = 'valid';
                        valid = true;
                    }
                }
            }
            else {
                document.getElementById('emailError').innerHTML = "<i class=\"fa fa-times-circle\"></i> E-Mail can't be empty";
                y[i].className = 'invalid';
                valid = false;
            }



            if (y[1].value != y[2].value) {
                y[1].className = " invalid";
                y[2].className = " invalid";
                document.getElementById('passwordError').innerHTML = "<i class=\"fa fa-times-circle\"></i> Passwords do not match";
                // and set the current valid status to false:
                valid = false;
            }
            else if (y[1].value == "" || y[2].value == "") {
                y[1].className = " invalid";
                y[2].className = " invalid";
                document.getElementById('passwordError').innerHTML = "<i class=\"fa fa-times-circle\"></i> Password can't be empty";
                // and set the current valid status to false:
                valid = false;
            }
            else {
                if (!CheckPassword(y[1].value) || !CheckPassword(y[2].value)) {
                    y[1].className = " invalid";
                    y[2].className = " invalid";
                    document.getElementById('passwordError').innerHTML = "<i class=\"fa fa-times-circle\"></i> Password is not in correct format";
                    // and set the current valid status to false:
                    valid = false;
                }
                else {
                    document.getElementById('passwordError').innerHTML = "";
                    y[1].className = "valid";
                    y[2].className = "valid";
                }
            }
        }
        if (currentTab == 1) {
            if (y[i].value == "") {
                y[i].className = 'invalid';
                valid = false;
            }
            checkCar($('#carInput').val());
            console.log(carResult);
        }
        if (currentTab == 3) {            
            document.getElementById('checkingMessage').style.display = "none";
            checkCreditCard(y[1].value, y[2].value, y[3].value);
            console.log('is valid: ' + creditCardResult);
            valid = creditCardResult;
            if (!valid) {
                document.getElementById('checkingMessage').style.display = "";
            }
            else {
                document.getElementById('checkingMessage').style.display = "none";
            }
            
        }
    }
    // If the valid status is true, mark the step as finished and valid:
    if (valid) {
        document.getElementsByClassName("step")[currentTab].className += " finish";
    }
    return valid; // return the valid status
}

function fixStepIndicator(n) {
    // This function removes the "active" class of all steps...
    var i, x = document.getElementsByClassName("step");
    for (i = 0; i < x.length; i++) {
        x[i].className = x[i].className.replace(" active", "");
    }
    //... and adds the "active" class to the current step:
    x[n].className += " active";
}