window.saveAsFile = function(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

// Quicker logout
window.SubmitLogout = function() {
    document.getElementById('logoutForm').submit();
}

// Cookies
window.WriteCookie = function(name, value, days) {
    var expires;
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    }
    else {
        expires = "";
    }
    document.cookie = name + "=" + value + expires + "; path=/";
}
window.ReadCookie = function(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');(cname)
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

// Sweet Alert
window.swalConfirm = async function(title, text="") {
    let promise = Swal.fire({
            title: title,
            text: text,
            showCancelButton: true,
            confirmButtonText: `Yes`,
        })
        .then((result) => {
            if (result.isConfirmed) {
                return true;
            }
            else {
                return false;
            }
        });
      let result = await promise;
      return result;
}

// Toastr
window.notify = async function (text) {
    toastr.options.progressBar = true;
    toastr.options.timeOut = 2000;
    toastr.options.extendedTimeOut = 2000;
    toastr.success(text);
    return true;
}
window.notifyError = async function (text) {
    toastr.options.progressBar = true;
    toastr.options.timeOut = 2000;
    toastr.options.extendedTimeOut = 2000;
    toastr.error(text);
    return true;
}
window.notifyInfo = async function (text) {
    toastr.options.progressBar = true;
    toastr.options.timeOut = 2000;
    toastr.options.extendedTimeOut = 2000;
    toastr.info(text);
    return true;
}

// bootstrap modal
window.modalClose = function() {
    $('#mainModal').modal('hide');
}

// scroll top
window.ScrollTop = function() {
    document.documentElement.scrollTop = 0;
}

// download file
// download file
function BlazorDownloadFile(filename, contentType, bytes) {
    const uint8Array = new Uint8Array(bytes);
    // Create the URL
    const file = new File([uint8Array], filename, { type: contentType });
    const exportUrl = URL.createObjectURL(file);

    // Create the <a> element and click on it
    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = filename;
    a.target = "_self";
    a.click();

    // Release memory
    URL.revokeObjectURL(exportUrl);
}
// Convert a base64 string to a Uint8Array. This is needed to create a blob object from the base64 string.
function b64ToUint6(nChr) {
    return nChr > 64 && nChr < 91 ? nChr - 65 : nChr > 96 && nChr < 123 ? nChr - 71 : nChr > 47 && nChr < 58 ? nChr + 4 : nChr === 43 ? 62 : nChr === 47 ? 63 : 0;
}
function base64DecToArr(sBase64, nBlocksSize) {
    var
        sB64Enc = sBase64.replace(/[^A-Za-z0-9\+\/]/g, ""),
        nInLen = sB64Enc.length,
        nOutLen = nBlocksSize ? Math.ceil((nInLen * 3 + 1 >> 2) / nBlocksSize) * nBlocksSize : nInLen * 3 + 1 >> 2,
        taBytes = new Uint8Array(nOutLen);

    for (var nMod3, nMod4, nUint24 = 0, nOutIdx = 0, nInIdx = 0; nInIdx < nInLen; nInIdx++) {
        nMod4 = nInIdx & 3;
        nUint24 |= b64ToUint6(sB64Enc.charCodeAt(nInIdx)) << 18 - 6 * nMod4;
        if (nMod4 === 3 || nInLen - nInIdx === 1) {
            for (nMod3 = 0; nMod3 < 3 && nOutIdx < nOutLen; nMod3++, nOutIdx++) {
                taBytes[nOutIdx] = nUint24 >>> (16 >>> nMod3 & 24) & 255;
            }
            nUint24 = 0;
        }
    }
    return taBytes;
}

// blink tr after moving item x times
window.blink = function (trId, times) {
    var tr = document.getElementById(trId);

    for (x = 0; x < times; x++) {
        blinkOnce(x);
    }

    function blinkOnce(delay) {
        delay *= 2;
        window.setTimeout(function () {
            $(tr).find('td').addClass('greyBg')
        }, 100 * delay);
        window.setTimeout(function () {
            $(tr).find('td').removeClass('greyBg')
        }, 100 * (delay + 1));
    }
}
