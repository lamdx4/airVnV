import { useEffect } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { ChatContainer } from "../features/chat/components/ChatContainer";
import { ChatProvider, useChat } from "../features/chat/context/ChatContext";
import { useInitConversation } from "../features/chat/hooks/useInitConversation";

const MessagesContent = () => {
  const [searchParams] = useSearchParams();
  const propertyId = searchParams.get("propertyId");

  const { setActiveConversationId } = useChat();
  const navigate = useNavigate();
  const { mutate: initConversation } = useInitConversation();

  useEffect(() => {
    if (propertyId) {
      // Create or get existing conversation for this property
      initConversation(
        { propertyId },
        {
          onSuccess: (id) => {
            setActiveConversationId(id);
            // Remove search params from URL so it doesn't re-trigger on refresh
            navigate('/messages', { replace: true });
          },
        },
      );
    }
  }, [
    propertyId,
    setActiveConversationId,
    initConversation,
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
