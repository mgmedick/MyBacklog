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
                <button v-for="(userGameAccount, userGameAccountIndex) in importgamesvm.userGameAccounts" type="button" class="btn btn-outline-dark d-flex" @click="onImportGamesClick(userGameAccount.id)" :disabled="userGameAccount.isImportRunning">
                    <font-awesome-icon :icon="getIconClass(userGameAccount.gameAccountTypeID)" size="xl"/>
                    <span class="mx-auto">{{ (userGameAccount.isImportRunning ? 'Importing ' : 'Import ') + userGameAccountType.gameAccountTypeName + ' games' }}</span>
                    <font-awesome-icon v-if="userGameAccount.isImportRunning" icon="fa-solid fa-spinner" size="xl"/>
                </button>
            </div>
        </div>
    </div>
</template>
<script>
    import { Toast } from 'bootstrap';
    
    export default {
        name: "ImportGames",
        props: {
            importgamesvm: Object
        },
        data() {
            return {
                successToast: {},
                successMessage: '',
                errorMessages: [],
                errorToast: {}                
            }
        },
        computed: {
        },
        created: function () {
        },        
        mounted: function () {
            var that = this;
            that.successToast = new Toast(that.$refs.successtoast);
            that.errorToast = new Toast(that.$refs.errortoast);

            if (that.importgamesvm.authSuccess != null) {
                if (that.importgamesvm.authSuccess) {
                    var userGameAccount = that.importgamesvm.userGameAccounts.find(i => i.gameAccountTypeID == that.importgamesvm.authGameAccountTypeID);
                    that.importGames(userGameAccount);
                } else {            
                    that.errorMessages = ["Error authorizing account"];
                    if (that.errorMessages.length > 0) {
                        that.$nextTick(function() {
                            that.$refs.errortoasts?.forEach(el => {
                                new Toast(el).show();
                            });
                        }); 
                    } 
                }
            }            
        },        
        methods: {
            getIconClass: function (gameAccountTypeID) {
                var iconClass = '';

                switch (gameAccountTypeID) {
                    case 1:
                        iconClass = 'fa-brands fa-steam text-color-blue';
                        break;
                    case 2:
                        iconClass = 'fa-brands fa-xbox text-color-green';
                        break;
                }

                return iconClass;
            },
            onImportGamesClick(userGameAccount) {
                this.importGames(userGameAccount);
            },
            importGames(userGameAccount) {
                var that = this;
                userGameAccount.isImportRunning = true;

                return axios.post('/User/ImportGamesFromUserGameAccount', null,{ params: { userGameAccountID: userGameAccount.id } })
                    .then((res) => {
                        if (res.data.success) {
                            that.successMessage = "Successfully imported " + userGameAccount.name + " games"
                            that.successToast.show();
                        } else {
                            that.errorMessages = ["Error importing " + userGameAccount.name + " games"];
                            if (that.errorMessages.length > 0) {
                                that.$nextTick(function() {
                                    that.$refs.errortoasts?.forEach(el => {
                                        new Toast(el).show();
                                    });
                                }); 
                            }                           
                        }
                        userGameAccount.isImportRunning = false;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },            
        }
    };
</script>


