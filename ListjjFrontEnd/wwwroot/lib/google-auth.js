let globalAuthService = null;

function initialize(clientId, authService) {
    globalAuthService = authService;
    google.accounts.id.initialize({ client_id: clientId, callback: callback });
}

function googlePrompt() {
    google.accounts.id.prompt((notification) => {
        if (notification.isNotDisplayed() || notification.isSkippedMoment()) {
            console.info(notification.getNotDisplayedReason());
            console.info(notification.getSkippedReason());
        }
    });
}

function callback(googleResponse) {
    globalAuthService.invokeMethodAsync("GoogleLogin", { ClientId: googleResponse.clientId, SelectedBy: googleResponse.select_by, Credential: googleResponse.credential });
}