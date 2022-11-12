var currentTab = 0; // Current tab is set to be the first tab (0)
showTab(currentTab); // Display the current tab

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
        document.getElementById("regForm").submit();
        return false;
    }
    // Otherwise, display the correct tab:
    showTab(currentTab);
}

function validateEmail(mail) {
    if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail)) {
        return (true)
    }
    return (false)
}

function CheckPassword(inputtxt) {
    if (/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9])(?!.*\s).{8,15}$/.test(inputtxt)) {
        return (true)
    }
    return (false)
} 

function validateForm() {
    // This function deals with validation of the form fields
    var x, y, i, valid = true;
    x = document.getElementsByClassName("tab");
    y = x[currentTab].getElementsByTagName("input");
    // A loop that checks every input field in the current tab: 
    for (i = 0; i < y.length; i++) {
        // If a field is empty...
        if (y[i].value == "") {
            // add an "invalid" class to the field:
            y[i].className += " invalid";
            document.getElementById('emailError').innerHTML = "E-Mail can't be empty";
            document.getElementById('passwordError').innerHTML = "Password can't be empty";
            // and set the current valid status to false:
            valid = false;
        }
        else {
            document.getElementById('emailError').innerHTML = ""; 
            document.getElementById('passwordError').innerHTML = "";
        }
        if (currentTab == 0) {
            if (!validateEmail(y[0].value)) {
                y[i].className += " invalid";
                document.getElementById('emailError').innerHTML = "E-Mail is not in correct format"; 
                // and set the current valid status to false:
                valid = false;
            }
            else {
                document.getElementById('emailError').innerHTML = "";               
            }
                
            if (y[1].value != y[2].value) {
                y[1].className += " invalid";
                y[2].className += " invalid";
                document.getElementById('passwordError').innerHTML = "Passwords do not match";
                // and set the current valid status to false:
                valid = false;
            }
            else {
                if (!CheckPassword(y[1].value) || !CheckPassword(y[2].value)) {
                    y[1].className += " invalid";
                    y[2].className += " invalid";
                    document.getElementById('passwordError').innerHTML = "Password is not in correct format";
                    // and set the current valid status to false:
                    valid = false;
                }
                else {
                    document.getElementById('passwordError').innerHTML = "";
                }        
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