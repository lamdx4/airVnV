import { initializeApp } from "firebase/app";
import { getMessaging, getToken } from "firebase/messaging";

const firebaseConfig = {
  apiKey: "AIzaSyDUXPzYj0nh9-zydomOCNJE3T86os4Vs5c",
  authDomain: "airvnv-1e7b7.firebaseapp.com",
  projectId: "airvnv-1e7b7",
  storageBucket: "airvnv-1e7b7.firebasestorage.app",
  messagingSenderId: "748228708558",
  appId: "1:748228708558:web:e8d316d4f00a0a9562a89f",
  measurementId: "G-LW47TKG9GE"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const messaging = typeof window !== 'undefined' ? getMessaging(app) : null;

export const getFCMToken = async (): Promise<string | null> => {
  if (!messaging) return null;
  
  try {
    // 1. Yêu cầu trình duyệt cấp quyền thông báo
    const permission = await Notification.requestPermission();
    if (permission !== 'granted') {
      console.warn("Quyền thông báo bị từ chối.");
      return null;
    }

    // 2. Lấy Token
    // TODO: Thay VAPID Key thực tế của bạn vào đây từ Firebase Console nếu cần
    const token = await getToken(messaging, {
      vapidKey: import.meta.env.VITE_FIREBASE_VAPID_KEY || undefined
    });

    if (token) {
      return token;
    } else {
      console.warn("Không thể lấy FCM Token.");
      return null;
    }
  } catch (error) {
    console.error("Lỗi khi lấy FCM Token:", error);
    return null;
  }
};

export { app, messaging };
