const toggleButton = document.querySelector(".toggle-button");
toggleButton.addEventListener("click", function () {
    expandSidebar();
    showHover();
});
function expandSidebar() {
    document.querySelector(".admin-sidebar").classList.toggle("admin-sidebar-collapsed");
    //	document.querySelector(".admin-sidebar-item").classList.toggle("admin-sidebar-item-icons");
    //	document.querySelector(".admin-sidebar-link").classList.toggle("admin-sidebar-link-icons");
    let keepSideBar = document.querySelector(".admin-sidebar-collapsed");
    if (keepSideBar.length === 1) {
        localStorage.setItem("keepSideBar", "true");
        alert("hoy");
    }
    else {
        localStorage.removeItem("keepSideBar");
        alert("hoyna");
    }
}
/**
 * check local storage for keep sidebar
 */
function showStoredSidebar() {
    if (localStorage.getItem("keepSidebar") === "true") {
        document.querySelector(".admin-sidebar").classList.add("admin-sidebar-collapsed");
        showHover();
    }
}

showStoredSidebar(); // show sidebar if stored in local storage