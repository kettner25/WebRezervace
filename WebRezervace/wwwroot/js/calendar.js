var indexKalendare = 0;

window.onload = function () {
    document.getElementById("kalendar"+indexKalendare).style.display = "block";
}

function KalendarZpet() {
    if (indexKalendare > 0){
        document.getElementById("kalendar"+indexKalendare).style.display = "none";
        indexKalendare--;
        document.getElementById("kalendar"+indexKalendare).style.display = "block";
    }
}
function KalendarDalsi() {
    if (indexKalendare < 3) {
        document.getElementById("kalendar"+indexKalendare).style.display = "none";
        indexKalendare++;
        document.getElementById("kalendar"+indexKalendare).style.display = "block";
    }
}


