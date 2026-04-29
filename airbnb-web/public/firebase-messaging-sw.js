// Dành cho xử lý Push Notification ngầm (Background notifications)
importScripts('https://www.gstatic.com/firebasejs/10.8.0/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/10.8.0/firebase-messaging-compat.js');

const firebaseConfig = {
  apiKey: "AIzaSyDUXPzYj0nh9-zydomOCNJE3T86os4Vs5c",
  authDomain: "airvnv-1e7b7.firebaseapp.com",
  projectId: "airvnv-1e7b7",
  storageBucket: "airvnv-1e7b7.firebasestorage.app",
  messagingSenderId: "748228708558",
  appId: "1:748228708558:web:e8d316d4f00a0a9562a89f",
  measurementId: "G-LW47TKG9GE"
};

firebase.initializeApp(firebaseConfig);
const messaging = firebase.messaging();

messaging.onBackgroundMessage((payload) => {
  console.log('[firebase-messaging-sw.js] Nhận thông báo ngầm: ', payload);

  const notificationTitle = payload.notification.title;
  const notificationOptions = {
    body: payload.notification.body,
    icon: '/favicon.svg'
  };

  self.registration.showNotification(notificationTitle, notificationOptions);
});
