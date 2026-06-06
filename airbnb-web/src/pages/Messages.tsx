import { useEffect, useRef } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { ChatContainer } from "../features/chat/components/ChatContainer";
import { ChatProvider, useChat } from "../features/chat/context/ChatContext";
import { useInitConversation } from "../features/chat/hooks/useInitConversation";

const MessagesContent = () => {
  const [searchParams] = useSearchParams();
  const propertyId = searchParams.get("propertyId");

  const { setActiveConversationId } = useChat();
  const navigate = useNavigate();
  const { mutateAsync: initConversationAsync } = useInitConversation();
  const initAttemptedRef = useRef<string | null>(null);

  useEffect(() => {
    if (propertyId && initAttemptedRef.current !== propertyId) {
      initAttemptedRef.current = propertyId;
      // Create or get existing conversation for this property
      initConversationAsync({ propertyId })
        .then((id) => {
          setActiveConversationId(id);
          // Remove search params from URL so it doesn't re-trigger on refresh
          navigate('/messages', { replace: true });
        })
        .catch((err) => {
          console.error("Failed to init conversation:", err);
          initAttemptedRef.current = null; // Reset on failure
        });
    }
  }, [
    propertyId,
    setActiveConversationId,
    initConversationAsync,
    navigate,
  ]);

  return <ChatContainer />;
};

export default function Messages() {
  return (
    <ChatProvider>
      <MessagesContent />
    </ChatProvider>
  );
}
