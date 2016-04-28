$(document).ready(function () {
    window.nativeHost.ready();
    //Wait 5 seconds then check for available updates
    setTimeout(function () {
        window.nativeHost.checkForUpdates();
    }, 5000);
});

window.updateAvailable = function () {
    $('#announce')
        .fadeIn("fast")
        .html('Update available! <button type="button" class="btn btn-success" onclick="performUpdate();">Update</button>');
};

window.performUpdate = function () {
    $('#announce').html('Updating.. Application will restart..');
    window.nativeHost.performUpdate();
};