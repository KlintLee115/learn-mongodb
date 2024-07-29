"use client"

import { signIn, useSession } from "next-auth/react"
import Image from "next/image"

export default function Header() {

    const session = useSession()
    const userImg = session.data?.user?.image

    return <header className="flex justify-between items-center">
        <h3>Forms</h3>
        <input type="text" placeholder="Search" className="bg-gray-200 rounded-md py-4 px-3 placeholder-slate-500" />
        {
            userImg ? <Image src={userImg} alt="profile pic" height={50} width={50} /> : <button onClick={() => signIn('google')}>Login</button>
        }

    </header>
}