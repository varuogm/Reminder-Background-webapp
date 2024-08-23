import React, { useState, useEffect } from 'react';
import './App.css';

function App() {
  const [isSubscribed, setIsSubscribed] = useState(false);

  useEffect(() => {
    if ('serviceWorker' in navigator && 'PushManager' in window) {
      navigator.serviceWorker.register('/service-worker.js')
        .then(registration => {
          console.log('Service Worker registered');
          // Check if the user is already subscribed
          registration.pushManager.getSubscription()
            .then(subscription => {
              setIsSubscribed(subscription !== null);
            });
        })
        .catch(err => {
          console.error('Service Worker registration failed:', err);
        });
    }
  }, []);
  
  const subscribeToNotifications = async () => {
    try {
      const registration = await navigator.serviceWorker.ready;
      const applicationServerKey = 'BGicA82QGmtXjMMwAp8xcVqRF84hTNZtFFC0NoBC6Lfs7aBAZdoxxz5P-vCWrl4nLFlurZuKKgzPa7yeCgGo570';
      const subscription = await registration.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: urlBase64ToUint8Array(applicationServerKey)
      });

      if (!subscription || !subscription.keys) {
        throw new Error('Failed to subscribe: Invalid subscription object');
      }

      const backendSubscription = {
        Endpoint: subscription.endpoint,
        P256dh: subscription.keys.p256dh,
        Auth: subscription.keys.auth,
        CreatedAt: new Date().toISOString()
      };

      const response = await fetch('http://localhost:7202/api/subscriptions', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(backendSubscription),
        credentials: 'include',
      });

      if (!response.ok) {
        throw new Error('Failed to send subscription to server');
      }

      setIsSubscribed(true);
    } catch (error) {
      console.error('Error subscribing to notifications:', error);
    }
  };

  const unsubscribeFromNotifications = async () => {
    try {
      const registration = await navigator.serviceWorker.ready;
      const subscription = await registration.pushManager.getSubscription();

      if (subscription) {
        await subscription.unsubscribe();

        const response = await fetch('http://localhost:7202/api/subscriptions', {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            Endpoint: subscription.endpoint,
          }),
          credentials: 'include',
        });

        if (!response.ok) {
          throw new Error('Failed to send unsubscription to server');
        }

        setIsSubscribed(false);
      }
    } catch (error) {
      console.error('Error unsubscribing from notifications:', error);
    }
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>Notification Subscription</h1>
        {isSubscribed ? (
          <>
            <p>You are subscribed to notifications.</p>
            <button onClick={unsubscribeFromNotifications}>Unsubscribe from Notifications</button>
          </>
        ) : (
          <button onClick={subscribeToNotifications}>Subscribe to Notifications</button>
        )}
      </header>
    </div>
  );
}

// Ensure urlBase64ToUint8Array function is defined
function urlBase64ToUint8Array(base64String) {
  const padding = '='.repeat((4 - base64String.length % 4) % 4);
  const base64 = (base64String + padding)
    .replace(/\-/g, '+')
    .replace(/_/g, '/');

  try {
    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
      outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
  } catch (e) {
    console.error('Invalid base64 string:', e);
    throw new Error('Invalid base64 string');
  }
}

export default App;