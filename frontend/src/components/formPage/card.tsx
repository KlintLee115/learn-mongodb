import { HTMLAttributes } from "react";

export default function Card({ children, cssProps }: { children: JSX.Element, cssProps?: HTMLAttributes<HTMLDivElement>["className"] }) {
    return <div className={`bg-white p-4 rounded-sm mb-4 ${cssProps}`}>
        {children}
    </div>
}