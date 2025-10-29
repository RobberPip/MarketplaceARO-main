import clsx from 'clsx';
import React from 'react';

export const StepIndicator: React.FC<{
  steps: Array<number>;
  currentStep: number;
}> = ({ steps, currentStep }) => {
  return (
    <div className="flex space-x-2 justify-center align-items-center">
      {steps.map((num, index) => (
        <React.Fragment key={`step-${num}`}>
          <div className="flex flex-column align-items-center">
            <div
              className={clsx(
                'w-2rem h-2rem border-circle flex align-items-center justify-content-center text-sm transition-colors transition-duration-150',
                currentStep === num && 'bg-gray-900 text-white',
                currentStep > num && 'border-1 border-gray-900 text-gray-900',
                currentStep < num && 'border-1 border-gray-600 text-gray-600 bg-white',
              )}
            >
              {num}
            </div>
          </div>
          {index < steps.length - 1 && (
            <div style={{ height: '1px' }} className={clsx('w-2rem bg-gray-600', currentStep > num && 'bg-gray-900')} />
          )}
        </React.Fragment>
      ))}
    </div>
  );
};
