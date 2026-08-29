"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/trip")
    .withAutomaticReconnect()
    .build();

// ============================================
// Notifications
// ============================================
function showNewRideNotification() {
    const container = document.getElementById("notificationContainer");
    if (!container) return;

    const notification = document.createElement("div");
    notification.className = "alert alert-success shadow";
    notification.style.minWidth = "300px";
    notification.innerHTML = "🚕 <strong>New Ride!</strong><br>A new ride request is available.";

    container.appendChild(notification);
    setTimeout(() => { notification.remove(); }, 4000);
}

function showRideAcceptedNotification(data) {
    const container = document.getElementById("notificationContainer");
    if (!container) return;

    const notification = document.createElement("div");
    notification.className = "alert alert-success shadow";
    notification.style.minWidth = "300px";
    notification.innerHTML = `🚕 <strong>Ride Accepted!</strong><br>${data.driverName} accepted your ride.`;

    container.appendChild(notification);
    setTimeout(() => { notification.remove(); }, 4000);
}

function showRideCancelledNotification(data) {
    const container = document.getElementById("notificationContainer");
    if (!container) return;

    const notification = document.createElement("div");
    notification.className = "alert alert-danger shadow";
    notification.style.minWidth = "300px";
    notification.innerHTML = "❌ <strong>Ride Cancelled</strong><br>The passenger has cancelled the ride.";

    container.appendChild(notification);
    setTimeout(() => { notification.remove(); }, 4000);
}

function showRideStartedNotification(data) {
    const container = document.getElementById("notificationContainer");
    if (!container) return;

    const notification = document.createElement("div");
    notification.className = "alert alert-info shadow";
    notification.style.minWidth = "300px";

   
    notification.innerHTML =
        `🚀 <strong>Ride Started!</strong><br>` +
        `<strong>Time:</strong> ${data.startTime}<br>` +
        `Have a safe and nice trip!`;

    container.appendChild(notification);
    setTimeout(() => { notification.remove(); }, 4000);
}

function showRideCompletedNotification(data) {
    const container = document.getElementById("notificationContainer");
    if (!container) return;

    const notification = document.createElement("div");
    notification.className = "alert alert-success shadow";
    notification.style.minWidth = "300px";

    
    notification.innerHTML =
        `🏁 <strong>Arrived Safely!</strong><br>` +
        `<hr class="my-2">` +
        `<strong>Ended At:</strong> ${data.endTime}<br>` +
        `<strong>Duration:</strong> ${data.duration}<br>` +
        `<strong>Final Fare:</strong> ${data.actualFare} EGP`;

    container.appendChild(notification);

    setTimeout(() => { notification.remove(); }, 6000);
}

// ============================================
// UI Updates
// ============================================


function addRideToDashboard(ride) {
    const container = document.getElementById("availableRidesContainer");
    if (!container) return;

    document.getElementById("noRequestsMessage")?.remove();

    if (container.querySelector(`[data-ride-id="${ride.rideId}"]`)) return;

    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    const tokenInput = tokenElement ? `<input type="hidden" name="__RequestVerificationToken" value="${tokenElement.value}" />` : '';

    const card = document.createElement("div");
    card.className = "col-md-6 mb-3";
    card.dataset.rideId = ride.rideId;

    card.innerHTML = `
        <div class="card shadow-sm h-100">
            <div class="card-body">
                <h5 class="card-title text-primary">
                    <i class="bi bi-geo-alt"></i> ${ride.distanceInKm} km Trip
                </h5>
                <p class="text-dark fw-bold mb-2">
                    <i class="bi bi-person"></i> ${ride.passengerName}
                </p>
                <p class="mb-1">
                    <strong>From:</strong> ${ride.pickupLocation}
                </p>
                <p class="mb-3">
                    <strong>To:</strong> ${ride.dropoffLocation}
                </p>

                <div class="d-flex justify-content-between align-items-center">
                    <span class="fw-bold text-success fs-5">
                        ${ride.estimatedFare} EGP
                    </span>
                    <form action="/Trips/Accept" method="post">
                        ${tokenInput}
                        <input type="hidden" name="rideId" value="${ride.rideId}" />
                        <button type="submit" class="btn btn-success">
                            <i class="bi bi-check2-circle"></i> Accept
                        </button>
                    </form>
                </div>
            </div>
            <div class="card-footer text-muted text-center" style="font-size: 0.85rem;">
                Requested at ${ride.requestTime}
            </div>
        </div>
    `;
    container.prepend(card);
}

function removeRideFromDashboard(rideId) {
    const container = document.getElementById("availableRidesContainer");
    if (!container) return;

    
    const ride = container.querySelector(`[data-ride-id="${rideId}"]`);
    if (ride) {
        ride.remove();
    }

    const remainingRides = container.querySelectorAll("[data-ride-id]");
    if (remainingRides.length === 0) {
        const message = document.createElement("div");
        message.className = "col-12";
        message.id = "noRequestsMessage";
        message.innerHTML = `
            <div class="alert alert-secondary text-center">
                No new ride requests at the moment.
            </div>
        `;
        container.appendChild(message);
    }
}

function updatePassengerActiveRide(data) {
    const rideCard = document.getElementById("passengerRideCard");
    if (!rideCard) return;
    if (rideCard.dataset.rideId != data.rideId) return;

    const statusBadges = rideCard.querySelectorAll(".ride-status");
    statusBadges.forEach(badge => { badge.textContent = data.status; });

    const driverInfoContainer = rideCard.querySelector(".driver-info");
    if (driverInfoContainer) {
        driverInfoContainer.innerHTML = `
            <div class="p-3 bg-light rounded border">
                <p class="mb-1 text-success fw-bold">
                    <i class="bi bi-person-check-fill"></i> Captain: ${data.driverName}
                </p>
                <p class="mb-1">
                    <i class="bi bi-telephone-fill text-primary"></i> Phone: ${data.driverPhone}
                </p>
                <p class="mb-0">
                    <i class="bi bi-car-front-fill text-danger"></i> Car: <strong>${data.carColor} ${data.carModel}</strong>
                    <br />
                    <span class="text-muted">Plate: ${data.carPlate}</span>
                </p>

            </div>
        `;
    }
}

function updatePassengerRideStarted(data) {
    const rideCard = document.getElementById("passengerRideCard");

    if (!rideCard) return;
    if (rideCard.dataset.rideId != data.rideId) return;

   
    const statusBadges = rideCard.querySelectorAll(".ride-status");
    statusBadges.forEach(badge => {
        badge.textContent = data.status;
        badge.className = "badge bg-info text-dark ride-status";
    });

   
    const cancelForm = document.getElementById("cancelRideForm");
    if (cancelForm) {
        cancelForm.remove();
    }

 
    const infoColumn = rideCard.querySelector(".col-md-8");

    
    if (infoColumn && !document.getElementById("liveStartTime")) {
        const timeElement = document.createElement("p");
        timeElement.id = "liveStartTime";
        timeElement.className = "mb-3 text-info fw-bold fs-5";

        timeElement.innerHTML = `<i class="bi bi-clock-history"></i> Started at: ${data.startTime}`;


        infoColumn.insertBefore(timeElement, infoColumn.firstChild);
    }
}

function updatePassengerRideCompleted(data) {
    const rideCard = document.getElementById("passengerRideCard");
    if (rideCard && rideCard.dataset.rideId == data.rideId) {
        const statusBadges = rideCard.querySelectorAll(".ride-status");
        statusBadges.forEach(badge => {
            badge.textContent = "Completed";
            badge.className = "badge bg-success text-white ride-status";
        });
    }
    setTimeout(() => { window.location.reload(); }, 3000);
}

// ============================================
// SignalR Events
// ============================================
connection.on("NewRide", ride => {
    console.log("🚕 NEW RIDE RECEIVED:", ride);
    showNewRideNotification();
    addRideToDashboard(ride);
});

connection.on("RideAccepted", data => {
    console.log("✅ RIDE ACCEPTED:", data);
    showRideAcceptedNotification(data);
    updatePassengerActiveRide(data);
});

connection.on("RideCancelled", data => {
    console.log("❌ RIDE CANCELLED:", data);
    showRideCancelledNotification(data);

    const activeRideCard = document.getElementById("driverActiveRideCard");
    if (activeRideCard && activeRideCard.dataset.rideId == data.rideId) {
        const actionForms = activeRideCard.querySelectorAll("form");
        actionForms.forEach(form => form.remove());

        const statusBadges = activeRideCard.querySelectorAll(".ride-status");
        statusBadges.forEach(badge => {
            badge.textContent = "Cancelled";
            badge.className = "badge bg-danger text-white ride-status";
        });

        setTimeout(() => { window.location.reload(); }, 3000);
    } else {
        removeRideFromDashboard(data.rideId);
    }
});

connection.on("RemoveAvailableRide", data => {
    console.log("🗑️ RIDE REMOVED FROM DASHBOARD:", data);
    removeRideFromDashboard(data.rideId);
});

connection.on("RideStarted", data => {
    console.log("🚀 RIDE STARTED:", data);
    showRideStartedNotification(data);
    updatePassengerRideStarted(data);
});

connection.on("RideCompleted", data => {
    console.log("🏁 RIDE COMPLETED:", data);
    showRideCompletedNotification(data);
    updatePassengerRideCompleted(data);
});

// ============================================
// Start Connection
// ============================================
async function startConnection() {
    try {
        await connection.start();
        console.log("✅ SignalR Connected");
    } catch (error) {
        console.error("❌ SignalR Error:", error);
        setTimeout(startConnection, 5000);
    }
}

startConnection();