import React from 'react';
import { motion } from 'framer-motion';

export const TypingIndicator: React.FC = () => {
  return (
    <div className="flex items-center gap-2 px-4 py-3 bg-[#F5F5F5] rounded-2xl rounded-tl-none w-fit shadow-sm">
      <div className="flex gap-1">
        {[0, 1, 2].map((i) => (
          <motion.div
            key={i}
            className="w-1.5 h-1.5 bg-slate-400 rounded-full"
            animate={{ 
              y: [0, -4, 0],
              opacity: [0.4, 1, 0.4] 
            }}
            transition={{ 
              duration: 0.8, 
              repeat: Infinity, 
              delay: i * 0.15,
              ease: "easeInOut"
            }}
          />
        ))}
      </div>
    </div>
  );
};
