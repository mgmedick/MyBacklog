<template>
    <div class="mx-auto">
        <h2 class="text-center mb-1">Import Games</h2>
        <div class="mx-auto" style="max-width:400px;">
            <div class="toast-container position-absolute sticky-top p-3 top-0 end-0" id="toastPlacement" style="margin-top: 70px;"> 
                <div ref="errortoasts" v-for="errorMessage in errorMessages" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="d-flex">
                        <div class="toast-body">
                            <span>{{ errorMessage }}</span>
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                </div>           
                <div ref="successtoast" class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="d-flex">
                        <div class="toast-body">
                            <span>{{ successMessage }}</span>
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                </div>
            </div>
            <div class="row g-2 justify-content-center">
                <button v-for="(userGameAccount, userGameAccountIndex) in userGameAccounts" type="button" class="btn btn-outline-dark d-flex" @click="onImportGamesClick(userGameAccount)">
                    <font-awesome-icon :icon="getIconClass(userGameAccount.gameAccountTypeID)" size="xl" style="color: #0a3169;" />
                    <span class="mx-auto">{{ userGameAccountType.isExpired ? 'Re-login to ' + userGameAccountType.gameAccountTypeName + ' account' : 'Import ' + userGameAccountType.gameAccountTypeName + ' games' }}</span>
                    <font-awesome-icon v-if="userGameAccount.isExpired" icon="fa-solid fa-circle-exclamation" size="xl" style="color: #d9534f;"/>
                    <font-awesome-icon v-else icon="fa-solid fa-circle-check" size="xl" style="color: #02b875;"/>
                </button>
                <button type="button" class="btn btn-primary" @click="onImportAllGamesClick" :disabled="userGameAccounts.filter(i => !i.isExpired).length == 0">Import all games</button>
            </div>
        </div>
    </div>
</template>
<script>
    import { Toast } from 'bootstrap';
    
    export default {
        name: "ImportGames",
        data() {
            return {
                userGameAccounts: [],
                confirmModal: {},                
                confirmModalText: '',
                successToast: {},
                successMessage: '',
                errorModal: {},
                errorMessages: [],
                errorToast: {}
            }
        },
        computed: {
        },
        created: function () {
            this.loadData();
        },        
        mounted: function () {       
            var that = this;
            that.errorModal = new Modal(that.$refs.errormodal);  
            that.confirmModal = new Modal(that.$refs.confirmmodal);  
        },
        methods: {
            loadData: function () {
                var that = this;
                this.loading = true;

                axios.get('/User/GetUserGameAccounts')
                    .then(res => {
                        that.userGameAccounts = res.data;
                        that.allgames = res.data.slice();
                        
                        if (that.orderByDesc) {
                            that.games = that.games.reverse();
                            that.allgames = that.allgames.reverse();
                        }

                        that.loading = false;
                        return res;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            getIconClass: function (gameAccountTypeID) {
                var iconClass = '';

                switch (gameAccountTypeID) {
                    case 1:
                        iconClass = 'fa-brands fa-steam';
                        break;
                    case 2:
                        iconClass = 'fa-brands fa-xbox';
                        break;
                }

                return iconClass;
            },      
            onXboxClick() {
                location.href = this.welcomevm.windowsLiveAuthUrl;
            },     
            onImportGamesClick(userGameAccount) {
                if (userGameAccount.isExpired){
                    location.href = userGameAccount.authUrl;
                } else {
                    this.importGames();
                }
            },       
            onImportAllGamesClick(userGameAccount) {
            },
                
        }
    };
</script>


