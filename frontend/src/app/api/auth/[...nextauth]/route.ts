import GoogleProvider from 'next-auth/providers/google'
import NextAuth from "next-auth/next"

if (!process.env.GOOGLE_OAUTH_CLIENT_ID) {
    throw new Error("Client ID must be provided");
}

if (!process.env.GOOGLE_OAUTH_CLIENT_SECRET) {
    throw new Error("Client secret must be provided");
}

const options = {
    providers: [
        GoogleProvider({
            clientId: process.env.GOOGLE_OAUTH_CLIENT_ID,
            clientSecret: process.env.GOOGLE_OAUTH_CLIENT_SECRET
        })
    ],
    secret: process.env.NEXTAUTH_SECRET,
}

const handler = NextAuth(options)

export { handler as GET, handler as POST }