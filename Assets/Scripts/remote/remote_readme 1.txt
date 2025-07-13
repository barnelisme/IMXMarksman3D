This explains how the user access the app fro home and how we prevent multiple users to share.
==============================================================================================
1- The software can be shared but the passkey must be unique per person
2- Each person must be connected to the internet to logon to the IMXD
3- After successful logon, the person can disconnect from the internet
4- The system will query the online platform/portal to verify the credetials and IP address.
5- We need to ensure that the passkey can only be used by a single IP address at a time.
6- To achieve this, when the user connects, we post the passkey, username, IP and MAC address to the portal where we save the IP and MAC bext to the pass_key.
7- If we detect another logon request with the same passkey but different IP and MAC we block the login process.
8- When the user buys the license/software from Imagistix, we assign him/her a passkey.