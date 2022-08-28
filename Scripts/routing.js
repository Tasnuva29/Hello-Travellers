function redirectIfLoggedIn(username) {
    if (username.trim().length > 0) {
        window.location.replace("/Home/Index");
    }
}


function redirectIfNotLoggedIn(username) {
    if (username.trim().length <= 0) {
        window.location.replace("/UserAuth/Login");
    }
}

function reloadIfNotLoggedIn(username) {
    if (username == null) {
        location.reload();
    }
}