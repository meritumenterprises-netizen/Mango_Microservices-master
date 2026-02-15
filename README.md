This setup is for the Windows 11 and Docker Desktop.

The main branch is Kubernetes2.

# 1. Enter these entries into the HOSTS file #
<img width="464" height="220" alt="image" src="https://github.com/user-attachments/assets/38b6c11b-2c39-4402-af8e-eb5122a9a0b0" />

# 2. In your network IP card define all the IP adresses #
192.168.0.134 and following according to the HOSTS file, or network addresses corresponding to your network-provided IP address, up to 10 addresses (because of Windows 11 limitations). Later on, you'll have to eventually change IP addresses in docker files to correspond with your network settings.
   
<img width="400" height="488" alt="image" src="https://github.com/user-attachments/assets/4fea1679-27fc-47c9-8fa5-2b6c10e27df2" />

# 3. Modify docker-compose.yml with IP addresses, defined earlier. #
