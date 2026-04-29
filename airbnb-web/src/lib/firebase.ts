import { initializeApp } from "firebase/app";
import { getMessaging, getToken } from "firebase/messaging";

const firebaseConfig = {
  apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
  authDomain: import.meta.env.VITE_FIREBASE_AUTH_DOMAIN,
  projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID,
  storageBucket: import.meta.env.VITE_FIREBASE_STORAGE_BUCKET,
  messagingSenderId: import.meta.env.VITE_FIREBASE_MESSAGING_SENDER_ID,
  appId: import.meta.env.VITE_FIREBASE_APP_ID,
  measurementId: import.meta.env.VITE_FIREBASE_MEASUREMENT_ID
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
