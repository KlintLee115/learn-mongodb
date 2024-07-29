"use client"

import { useSession } from "next-auth/react"
import Image from "next/image"

export default function FormPageHeader({ title }: { title: string }) {

    const session = useSession()
    const userImg = session.data?.user?.image

    return <header className="flex justify-between items-center">
        <h4>{title ?? "Untitled form"}</h4>
        <button className="bg-purple-800 px-3 py-2 rounded-md text-white">Send</button>
        {
            userImg ? <Image src={userImg} alt="profile pic" width={50} height={50} /> : <button>Login</button>
        }

    </header>
}