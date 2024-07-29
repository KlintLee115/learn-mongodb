import SessionProviderClientComponent from '@/components/SessionProviderClientComponent'
import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import { getServerSession } from 'next-auth';

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Create Next App",
  description: "Generated by create next app",
};

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {

  const session = await getServerSession()

  return (
    <html lang="en">
      <SessionProviderClientComponent session={session}>
        <body className={`${inter.className} px-5 py-2 relative min-h-screen`}>{children}</body>
      </SessionProviderClientComponent>
    </html>
  );
}
