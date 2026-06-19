import { useState, useEffect, useRef, useCallback } from 'react';
import type * as signalR from '@microsoft/signalr';
import { chatApi } from '../api/chatApi';

export type CallState = 'idle' | 'calling' | 'ringing' | 'connected';

export interface IncomingCallInfo {
  callerId: string;
  offer: RTCSessionDescriptionInit;
  isVideoCall: boolean;
}

const defaultRtcConfig: RTCConfiguration = {
  iceServers: [
    { urls: 'stun:stun.l.google.com:19302' },
    { urls: 'stun:stun1.l.google.com:19302' },
  ]
};

export const useWebRTC = (
  connection: signalR.HubConnection | null | undefined,
) => {
  const [callState, setCallState] = useState<CallState>('idle');
  const [incomingCall, setIncomingCall] = useState<IncomingCallInfo | null>(null);
  const [remoteStream, setRemoteStream] = useState<MediaStream | null>(null);
  
  const peerConnection = useRef<RTCPeerConnection | null>(null);
  const currentTargetId = useRef<string | null>(null);
  const pendingCandidates = useRef<RTCIceCandidateInit[]>([]);
  const rtcConfigRef = useRef<RTCConfiguration>(defaultRtcConfig);

  // Lấy cấu hình TURN/STUN từ Backend khi Hook mount
  useEffect(() => {
    const fetchCredentials = async () => {
      try {
        const data = await chatApi.getWebRtcCredentials();
        if (data && data.iceServers && data.iceServers.length > 0) {
          rtcConfigRef.current = { iceServers: data.iceServers };
          console.log('Fetched WebRTC ICE Servers successfully.');
        }
      } catch (e) {
        console.error('Failed to fetch WebRTC credentials, falling back to default STUN', e);
      }
    };
    fetchCredentials();
  }, []);

  // Hàm nội bộ để reset state
  const cleanupCallState = useCallback(() => {
    if (peerConnection.current) {
      peerConnection.current.close();
      peerConnection.current = null;
    }
    setCallState('idle');
    setIncomingCall(null);
    setRemoteStream(null);
    currentTargetId.current = null;
    pendingCandidates.current = [];
  }, []);

  // Cúp máy (Đang trong cuộc gọi hoặc đang gọi đi)
  const endCall = useCallback(async () => {
    if (connection && currentTargetId.current) {
      try {
        await connection.invoke('EndCall', currentTargetId.current);
      } catch (e) {
        console.error('Error ending call', e);
      }
    }
    cleanupCallState();
  }, [connection, cleanupCallState]);

  // Khởi tạo PeerConnection
  const createPeerConnection = useCallback((targetUserId: string) => {
    const pc = new RTCPeerConnection(rtcConfigRef.current);

    pc.onicecandidate = (event) => {
      if (event.candidate && connection) {
        connection.invoke('SendIceCandidate', targetUserId, event.candidate);
      }
    };

    pc.ontrack = (event) => {
      if (event.streams && event.streams[0]) {
        // Ép React re-render bằng cách tạo MediaStream mới (đổi reference) 
        // để video element cập nhật cả audio lẫn video track khi chúng được thêm vào tuần tự.
        setRemoteStream(new MediaStream(event.streams[0].getTracks()));
      }
    };

    pc.onconnectionstatechange = () => {
      if (pc.connectionState === 'disconnected' || pc.connectionState === 'failed' || pc.connectionState === 'closed') {
        endCall();
      }
    };

    return pc;
  }, [connection, endCall]);

  // Lắng nghe sự kiện từ SignalR
  useEffect(() => {
    if (!connection) return;

    const handleIncomingCall = (data: { callerId: string; offer: RTCSessionDescriptionInit; isVideoCall: boolean }) => {
      console.log('Incoming call from:', data.callerId, 'Video:', data.isVideoCall);
      setIncomingCall(data);
      setCallState('ringing');
    };

    const handleCallAnswered = async (data: { answererId: string; answer: RTCSessionDescriptionInit }) => {
      console.log('Call answered by:', data.answererId);
      if (peerConnection.current) {
        await peerConnection.current.setRemoteDescription(new RTCSessionDescription(data.answer));
        setCallState('connected');

        // Thêm các candidate đã xếp hàng (nếu có)
        for (const candidate of pendingCandidates.current) {
          try {
            await peerConnection.current.addIceCandidate(new RTCIceCandidate(candidate));
          } catch (e) {
            console.error('Error adding queued ice candidate', e);
          }
        }
        pendingCandidates.current = [];
      }
    };

    const handleReceiveIceCandidate = async (data: { senderId: string; candidate: RTCIceCandidateInit }) => {
      if (data.candidate) {
        if (peerConnection.current && peerConnection.current.remoteDescription) {
          try {
            await peerConnection.current.addIceCandidate(new RTCIceCandidate(data.candidate));
          } catch (e) {
            console.error('Error adding received ice candidate', e);
          }
        } else {
          // Xếp hàng các candidate đến sớm khi chưa set remoteDescription
          pendingCandidates.current.push(data.candidate);
        }
      }
    };

    const handleCallRejected = () => {
      console.log('Call was rejected by remote');
      cleanupCallState();
    };

    const handleCallEnded = () => {
      console.log('Call was ended by remote');
      cleanupCallState();
    };

    connection.on('IncomingCall', handleIncomingCall);
    connection.on('CallAnswered', handleCallAnswered);
    connection.on('ReceiveIceCandidate', handleReceiveIceCandidate);
    connection.on('CallRejected', handleCallRejected);
    connection.on('CallEnded', handleCallEnded);

    return () => {
      connection.off('IncomingCall', handleIncomingCall);
      connection.off('CallAnswered', handleCallAnswered);
      connection.off('ReceiveIceCandidate', handleReceiveIceCandidate);
      connection.off('CallRejected', handleCallRejected);
      connection.off('CallEnded', handleCallEnded);
    };
  }, [connection, cleanupCallState]);

  // Gọi đi
  const startCall = useCallback(async (targetUserId: string, localStream: MediaStream, isVideoCall: boolean) => {
    if (!connection) return;
    
    currentTargetId.current = targetUserId;
    const pc = createPeerConnection(targetUserId);
    peerConnection.current = pc;

    localStream.getTracks().forEach((track) => {
      pc.addTrack(track, localStream);
    });

    const offer = await pc.createOffer();
    await pc.setLocalDescription(offer);

    await connection.invoke('InitCall', targetUserId, offer, isVideoCall);
    setCallState('calling');
  }, [connection, createPeerConnection]);

  // Nghe máy
  const acceptCall = useCallback(async (localStream: MediaStream) => {
    if (!connection || !incomingCall) return;

    const targetUserId = incomingCall.callerId;
    currentTargetId.current = targetUserId;
    
    const pc = createPeerConnection(targetUserId);
    peerConnection.current = pc;

    localStream.getTracks().forEach((track) => {
      pc.addTrack(track, localStream);
    });

    await pc.setRemoteDescription(new RTCSessionDescription(incomingCall.offer));
    
    // Thêm các candidate đã xếp hàng (nếu Caller gửi quá nhanh)
    for (const candidate of pendingCandidates.current) {
      try {
        await pc.addIceCandidate(new RTCIceCandidate(candidate));
      } catch (e) {
        console.error('Error adding queued ice candidate', e);
      }
    }
    pendingCandidates.current = [];

    const answer = await pc.createAnswer();
    await pc.setLocalDescription(answer);

    await connection.invoke('AnswerCall', targetUserId, answer);
    
    setIncomingCall(null);
    setCallState('connected');
  }, [connection, incomingCall, createPeerConnection]);

  // Từ chối cuộc gọi đến
  const rejectCall = useCallback(async () => {
    if (!connection || !incomingCall) return;
    try {
      await connection.invoke('RejectCall', incomingCall.callerId);
    } catch (e) {
      console.error('Error rejecting call', e);
    }
    cleanupCallState();
  }, [connection, incomingCall, cleanupCallState]);




  return {
    callState,
    incomingCall,
    remoteStream,
    startCall,
    acceptCall,
    rejectCall,
    endCall
  };
};
