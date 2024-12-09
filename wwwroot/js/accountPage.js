function AccountViewModel() {
    this.email = ko.observable("");
    this.password = ko.observable("");
    this.confirmPassword = ko.observable("");
    this.errorMessage = ko.observable("");
    this.errormodalTitle = ko.observable("Error");  // Default title is "Error"
    this.modalCssClass = ko.observable("alert alert-danger");  // Default class is "alert-danger" for error
    this.userName = ko.observable("");
    this.selectedSecurityQuestion = ko.observable("");
    this.securityAnswer = ko.observable("");
    this.userEmail = ko.observable("");
    this.enteredPassword = ko.observable("");  // For password verification
    this.newPassword = {
        title: ko.observable(""),
        website: ko.observable(""),
        userName: ko.observable(""),
        password: ko.observable("")
    };

    this.searchQuery = ko.observable("");  // Observable for the search query

    // Observable array for storing password entries
    this.passwordEntries = ko.observableArray([]);

    // View state: login, register, home
    this.currentView = ko.observable("login");

    // Observable to control visibility of the password verification modal
    this.showVerifyPasswordModal = ko.observable(false);

    // Store the selected password entry to be verified
    this.passwordEntryToVerify = ko.observable(null);


    //Ttile and Button of Create and Edit Modal
    this.modalTitle = ko.observable('Create New Entry');
    this.modalButton = ko.observable('Save Entry');

    // Edit Mode Supportd
    this.editingPasswordEntry = ko.observable(null); // To store the password entry being edited
    this.isEditing = ko.observable(false); // To track if we are editing a password


    //Verify for Edit or View Password
    this.verifyForEdit= ko.observable(false);

    // Show Login View
    this.showLogin = () => {
        this.currentView("login");
        this.errorMessage(""); // Clear error message when switching views
    };

    // Show Register View
    this.showRegister = () => {
        this.currentView("register");
        this.errorMessage(""); // Clear error message when switching views
    };

    // Show Home View
    this.showHome = (email) => {
        this.userEmail(email);
        this.currentView("home");

        this.loadPasswordEntries();

        setInterval(() => {
            this.loadPasswordEntries();
        }, 5000);
    };

    // Login function
    this.login = () => {

        var emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;

        if (!emailRegex.test(this.email())) {
            this.showModal('error', 'Please enter a valid email address.', 'Error');
            this.showErrorModal();
            return;
        }
        var loginData = {
            Email: this.email(),
            Password: this.password()
        };

        $.ajax({
            url: '/Account/Login',
            type: 'POST',
            data: JSON.stringify(loginData),
            contentType: 'application/json',
            success: () => {
                this.showHome(this.email()); // If login is successful, show home page
            },
            error: () => {
                this.showModal('error', 'Invalid email or password.', 'Error');
                this.showErrorModal();
            }
        });
    };

    // Register function
    this.register = () => {
        if (this.password() !== this.confirmPassword()) {
            this.showModal('error', 'Passwords do not match.', 'Error');
            return;
        }

        var passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?":{}|<>])[A-Za-z\d!@#$%^&*(),.?":{}|<>]{8,}$/;

        if (!passwordRegex.test(this.password())) {
            this.showModal('error', 'Password must be at least 8 characters long, and include at least one lowercase letter, one uppercase letter, one number, and one special character.', 'Error');
            return;
        }

        var emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;

        if (!emailRegex.test(this.email())) {
            this.showModal('error', 'Please enter a valid email address.', 'Error');
            return;
        }

        var registerData = {
            Email: this.email(),
            PasswordHash: this.password(),
            Username: this.userName(),
            SecurityQuestion: this.selectedSecurityQuestion(),
            SecurityQAns: this.securityAnswer()
        };

        $.ajax({
            url: '/Account/Register',
            type: 'POST',
            data: JSON.stringify(registerData),
            contentType: 'application/json',
            success: () => {
                this.showHome(this.email()); // If register is successful, show home page
            },
            error: () => {
                this.showModal('error', 'A user with this Username or Email ID already exists. Please choose a different one and try again.', 'Error');
            }
        });
    };

    // Logout function
    this.logout = () => {
        $.ajax({
            url: '/Account/Logout',
            type: 'POST',
            success: () => {
                this.passwordEntries([]);
                this.userEmail('');
                this.showLogin(); // Return to login page on successful logout
            }
        });
    };

    // Save new password (AJAX to Save Entry)
    this.savePassword = () => {
        var passwordData = {
            Title: this.newPassword.title(),
            Website: this.newPassword.website(),
            Username: this.newPassword.userName(),
            Password: this.newPassword.password(),
            UserId: this.userEmail()
        };
        $.ajax({
            url: '/PasswordManager/Create',  // Adjust this route as needed
            type: 'POST',
            data: JSON.stringify(passwordData),
            contentType: 'application/json',
            success: (response) => {
                this.passwordEntries.push({
                    id: ko.observable(response.passwordEntryId),
                    title: ko.observable(this.newPassword.title()),
                    website: ko.observable(this.newPassword.website()),
                    userName: ko.observable(this.newPassword.userName()),
                    password: ko.observable(this.newPassword.password()),
                    displayPassword: ko.observable(false)
                });
                this.passwordEntries.valueHasMutated();

                this.clearPasswordForm();
                this.isEditing(false);
                $('#createPasswordModal').modal('hide');
                $('.modal-backdrop').remove();
                this.closeCreateEditPasswordModal();
                this.showModal('success', 'Password Entry Saved Successfully!', 'Success');


            },
            error: () => {
                this.clearPasswordForm();
                this.isEditing(false);
                $('#createPasswordModal').modal('hide');
                $('.modal-backdrop').remove();
                this.closeCreateEditPasswordModal();
                this.showModal('error', 'Save failed. A password with this name already exists. Please choose a different title.', 'Error');
                
            }
        });
    };

    this.addNewPassword = () => {
        this.clearPasswordForm();  // Ensure the form is clear
        this.modalTitle('Create New Entry');  // Update modal title
        this.modalButton('Save Entry');  // Update modal button text
        this.isEditing(false);  // Make sure we are in "create" mode
        $('#createPasswordModal').modal('show');  // Show the modal

    };

    // Update existing password (used when editing)
    this.updatePassword = () => {
        this.modalTitle('Edit Entry');
        this.modalButton('Update Password');
        var updatedPasswordData = {
            PasswordEntryId: this.editingPasswordEntry().id(),
            Title: this.newPassword.title(),
            Website: this.newPassword.website(),
            Username: this.newPassword.userName(),
            Password: this.newPassword.password(),
            UserId: this.userEmail()// Assuming each password entry has an ID
        };
        $.ajax({
            url: '/PasswordManager/Edit',  // Adjust the URL to your update endpoint
            type: 'PATCH',
            data: JSON.stringify(updatedPasswordData),
            contentType: 'application/json',
            success: () => {
                // Update the observable array with the new data
                this.editingPasswordEntry().title(this.newPassword.title());
                this.editingPasswordEntry().website(this.newPassword.website());
                this.editingPasswordEntry().userName(this.newPassword.userName());
                this.editingPasswordEntry().password(this.newPassword.password());

                // Reset the form and change mode back to create
                
                this.clearPasswordForm();
                this.isEditing(false);
                $('#createPasswordModal').modal('hide');
                $('.modal-backdrop').remove();
                this.closeCreateEditPasswordModal();
                this.showModal('success', 'Password Entry Updated Successfully!', 'Success');

            },
            error: () => {
                this.clearPasswordForm();
                this.isEditing(false);
                $('#createPasswordModal').modal('hide');
                $('.modal-backdrop').remove();
                this.closeCreateEditPasswordModal();
                this.showModal('error', 'A password entry with this title already exists. Please change the title and try again.', 'Error');
     
            }
        });
    };

    // Save or Update Entry based on mode
    this.saveOrUpdatePassword = () => {
        if (this.isEditing()) {
            this.updatePassword();
        } else {
            this.savePassword();
        }
    };

    // Function to clear the password form after save or update
    this.clearPasswordForm = () => {
        this.newPassword.title("");
        this.newPassword.website("");
        this.newPassword.userName("");
        this.newPassword.password("");
        
    };

    // Edit Entry function
    this.editPassword = (passwordEntry) => {
        this.verifyForEdit(true);
        this.showVerifyPasswordModal(true);
        this.modalTitle('Edit Entry');
        this.modalButton('Update Entry');
        // Fill the form with the password entry's current values
        this.newPassword.title(passwordEntry.title());
        this.newPassword.website(passwordEntry.website());
        this.newPassword.userName(passwordEntry.userName());
        this.newPassword.password(passwordEntry.password());

        // Set the entry being edited and switch to edit mode
        this.editingPasswordEntry(passwordEntry);
        this.isEditing(true);
    };

    // Delete password function
    this.deletePassword = (passwordEntry) => {
        var confirmDelete = window.confirm("Are you sure you want to delete this password entry?");
        var passwordTitle = passwordEntry.title();
        $.ajax({
            url: '/PasswordManager/Delete',  // Adjust to your delete endpoint
            type: 'POST',
            data: JSON.stringify({ Title: passwordTitle }),
            contentType: 'application/json',
            success: () => {
                this.passwordEntries.remove(passwordEntry);  // Remove the entry from the observable array
            },
            error: () => {
                this.errorMessage("Failed to delete the password entry.");
                this.showErrorModal();


            }
        });
    };


    this.deletePasswordConfirm = (passwordEntry) => {
        // Set the password entry to delete when the user confirms
        var passwordTitle = passwordEntry.title();
        console.log(passwordEntry);
        // Show the confirmation modal
        $('#deletePasswordModal').modal('show');

        // Set up the click event for the 'OK' button in the modal
        $('#confirmDeleteButton').off('click').on('click', () => {
            // If the user clicks OK, make the AJAX request
            $.ajax({
                url: '/PasswordManager/Delete',  // Adjust to your delete endpoint
                type: 'POST',
                data: JSON.stringify({ Title: passwordTitle }),
                contentType: 'application/json',
                success: () => {
                    //this.deletePassword(passwordEntry);  // Call the delete function (same as above)
                    this.passwordEntries.remove(passwordEntry);  // Remove the entry from the observable array
                    $('#deletePasswordModal').modal('hide');  // Hide the modal after successful deletion
                    this.showModal('success', 'Password Entry Deleted Successfully!', 'Success');

                },
                error: () => {
                    this.showModal('error', 'Failed to delete the password entry.', 'Error');
                    $('#deletePasswordModal').modal('hide');  // Hide the modal in case of error
                }
            });
        });
    };



    // Verify user password (to view password in the table)
    this.verifyPassword = (passwordEntry) => {
        // Store the entry that needs to be verified
        this.passwordEntryToVerify(passwordEntry);

        this.enteredPassword("");
        this.errorMessage("");  

        // Show the modal to enter the password
        this.showVerifyPasswordModal(true);
    };

    // Function to verify the password entered in the modal
    this.verifyUserPassword = () => {
        var enteredPassword = this.enteredPassword();

        $.ajax({
            url: '/Account/VerifyPassword',  // Adjust this route as needed
            type: 'POST',
            data: JSON.stringify({ password: enteredPassword }),
            contentType: 'application/json',
            success: () => {
                if (this.verifyForEdit()) {

                    this.showVerifyPasswordModal(false);
                    this.enteredPassword("");
                    this.errorMessage(""); 
                    $('#createPasswordModal').modal('show');

                }
                else {
                    // Show password after successful verification
                    var passwordEntry = this.passwordEntryToVerify();
                    passwordEntry.displayPassword(true);
                    this.enteredPassword("");
                    this.errorMessage(""); 
                    setTimeout(() => {
                        passwordEntry.displayPassword(false);  // Hide the password after 7 seconds
                    }, 7000);

                    // Hide the modal
                    this.showVerifyPasswordModal(false);
                }
            },
            error: () => {
                this.showVerifyPasswordModal(false);
                this.enteredPassword("");  // Clear entered password field

                this.showModal('error', 'Invalid password. Please try again', 'Error');
                
            }
        });
    };

    this.clearModalData = () => {
        this.enteredPassword("");  // Clear entered password field
        this.errorMessage("");  // Clear any error messages

    };

    this.closeVerifyPasswordModal = () => {
        this.verifyForEdit(false);
        this.showVerifyPasswordModal(false);  // Hide the modal
        this.clearModalData();  // Clear any residual data in the modal
        this.editingPasswordEntry(null);  // Clear the editing entry
        this.isEditing(false);  // Reset edit mode flag
    };


    var modalElement = document.getElementById("verifyPasswordModal");
    var modalInstance = new bootstrap.Modal(modalElement); 


    this.showVerifyPasswordModal.subscribe((newValue) => {
            // Show or hide modal based on the observable value
        if (newValue) {
            $('#verifyPasswordModal').modal('show');
        } else {
            $('#verifyPasswordModal').modal('hide');
            $('.modal-backdrop').remove();
            }
        
    });


    this.closeCreateEditPasswordModal = () => {
        this.verifyForEdit(false);
        this.clearPasswordForm();
        this.showVerifyPasswordModal(false);  // Hide the modal
        this.isEditing(false);
        this.modalTitle('Create New Entry');
        this.modalButton('Save Entry');
        $('#createPasswordModal').modal('hide');
        $('.modal-backdrop').remove();
       
    }



    this.showErrorModal = () => {
        $('#errorModal').modal('show');
    };

    // Function to clear the error message and close the modal
    this.clearErrorMessage = () => {
        this.errorMessage(""); // Clear the error message
        $('#errorModal').modal('hide'); // Close the modal
    };

    this.filteredPasswordEntries = ko.computed(() => {
        var query = this.searchQuery().toLowerCase();
        if (!query) {
            return this.passwordEntries();  // If no search query, return all entries
        } else {
            return ko.utils.arrayFilter(this.passwordEntries(), function (entry) {
                return entry.title().toLowerCase().includes(query) ||
                    entry.website().toLowerCase().includes(query) ||
                    entry.userName().toLowerCase().includes(query);
            });
        }
    });



    // Show Error or Success Modal with dynamic message
    this.showModal = (type, message, title) => {
        if (type === 'error') {
            this.errormodalTitle('Error');
            this.modalCssClass('alert alert-danger');  // Red background for error
        } else if (type === 'success') {
            this.errormodalTitle('Success');
            this.modalCssClass('alert alert-success');  // Green background for success
        }

        this.errorMessage(message);  // Set the message
        $('#errorModal').modal('show');  // Show the modal
    }; 

    // Function to load password entries for the user
    this.loadPasswordEntries = () => {
        $.ajax({
            url: '/PasswordManager/LoadEntries',
            type: 'GET',
            success: (data) => {
                
                // Update the passwordEntries observable array
                var entries = data.map(entry => {
                    return {
                        id: ko.observable(entry.passwordEntryId),
                        title: ko.observable(entry.title),
                        website: ko.observable(entry.website),
                        userName: ko.observable(entry.username),
                        password: ko.observable(entry.decryptedPassword),
                        displayPassword: ko.observable(false)
                    };
                });
                this.passwordEntries(entries);  // Populate the observable array with fetched entries
            },
            error: () => {
                this.errorMessage("Failed to load password entries.");
            }
        });
    };
}

ko.applyBindings(new AccountViewModel());
